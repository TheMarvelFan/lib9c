using Bencodex;
using Bencodex.Types;
using GraphQL;
using GraphQL.Client.Http;
using GraphQL.Client.Serializer.SystemTextJson;
using Libplanet.Action.State;
using Libplanet.Common;
using Libplanet.Crypto;
using Libplanet.Store.Trie;
using Libplanet.Types.Assets;
using Libplanet.Types.Blocks;
using Libplanet.Types.Consensus;

namespace Libplanet.Extensions.RemoteStates;

public class RemoteAccountState : IAccountState
{
    private readonly Uri _explorerEndpoint;
    private readonly GraphQLHttpClient _graphQlHttpClient;
    private readonly Address? _address;
    private readonly BlockHash? _offset;

    public RemoteAccountState(
        Uri explorerEndpoint,
        Address? address,
        BlockHash? offset)
    {
        _explorerEndpoint = explorerEndpoint;
        _address = address;
        _offset = offset;
        _graphQlHttpClient =
            new GraphQLHttpClient(_explorerEndpoint, new SystemTextJsonSerializer());
    }

    public ITrie Trie => throw new NotSupportedException();

    public IReadOnlyList<IValue?> GetStates(IReadOnlyList<Address> addresses)
        => addresses.Select(address => GetState(address)).ToList().AsReadOnly();

    public IValue? GetState(Address address)
    {
        var response = _graphQlHttpClient.SendQueryAsync<GetStatesResponseType>(
            new GraphQLRequest(
                @"query GetState($address: Address!, $accountAddress: Address!, $offsetBlockHash: ID!, $o)
                {
                    stateQuery
                    {
                        states(
                            address: $address,
                            accountAddress: $accountAddress,
                            offsetBlockHash: $offsetBlockHash)
                    }
                }",
                operationName: "GetState",
                variables: new
                {
                    address,
                    accountAddress = _address.ToString(),
                    offsetBlockHash = _offset is { } hash
                        ? ByteUtil.Hex(hash.ByteArray)
                        : null,
                })).Result;
        var codec = new Codec();
        return response.Data.StateQuery.States is { } state ? codec.Decode(state) : null;
    }

    public FungibleAssetValue GetBalance(Address address, Currency currency)
    {
        object? currencyInput = currency.TotalSupplyTrackable ? new
        {
            ticker = currency.Ticker,
            decimalPlaces = currency.DecimalPlaces,
            minters = currency.Minters?.Select(addr => addr.ToString()).ToArray(),
            totalSupplyTrackable = currency.TotalSupplyTrackable,
            maximumSupplyMajorUnit = currency.MaximumSupply.Value.MajorUnit,
            maximumSupplyMinorUnit = currency.MaximumSupply.Value.MinorUnit,
        } : new
        {
            ticker = currency.Ticker,
            decimalPlaces = currency.DecimalPlaces,
            minters = currency.Minters?.Select(addr => addr.ToString()).ToArray(),
            totalSupplyTrackable = currency.TotalSupplyTrackable,
        };
        var response = _graphQlHttpClient.SendQueryAsync<GetBalanceResponseType>(
            new GraphQLRequest(
            @"query GetBalance($owner: Address!, $currency: CurrencyInput!, $offsetBlockHash: ID!)
            {
                stateQuery
                {
                    balance(owner: $owner, currency: $currency, offsetBlockHash: $offsetBlockHash)
                    {
                        string
                    }
                }
            }",
            operationName: "GetBalance",
            variables: new
            {
                owner = address.ToString(),
                currency = currencyInput,
                offsetBlockHash = _offset is { } hash
                    ? ByteUtil.Hex(hash.ByteArray)
                    : throw new NotSupportedException(),
            })).Result;

        return FungibleAssetValue.Parse(currency, response.Data.StateQuery.Balance.String.Split()[0]);
    }

    public FungibleAssetValue GetTotalSupply(Currency currency)
    {
        object? currencyInput = currency.TotalSupplyTrackable ? new
        {
            ticker = currency.Ticker,
            decimalPlaces = currency.DecimalPlaces,
            minters = currency.Minters.Select(addr => addr.ToString()).ToArray(),
            totalSupplyTrackable = currency.TotalSupplyTrackable,
            maximumSupplyMajorUnit = currency.MaximumSupply.Value.MajorUnit,
            maximumSupplyMinorUnit = currency.MaximumSupply.Value.MinorUnit,
        } : new
        {
            ticker = currency.Ticker,
            decimalPlaces = currency.DecimalPlaces,
            minters = currency.Minters.Select(addr => addr.ToString()).ToArray(),
            totalSupplyTrackable = currency.TotalSupplyTrackable,
        };
        var response = _graphQlHttpClient.SendQueryAsync<GetTotalSupplyResponseType>(
            new GraphQLRequest(
                @"query GetTotalSupply(currency: CurrencyInput!, $offsetBlockHash: ID!)
            {
                stateQuery
                {
                    totalSupply(currency: $currency, offsetBlockHash: $offsetBlockHash)
                    {
                        string
                    }
                }
            }",
                operationName: "GetTotalSupply",
                variables: new
                {
                    currency = currencyInput,
                    offsetBlockHash = _offset is { } hash
                        ? ByteUtil.Hex(hash.ByteArray)
                        : throw new NotSupportedException(),
                })).Result;

        return FungibleAssetValue.Parse(currency, response.Data.StateQuery.TotalSupply.String.Split()[0]);
    }

    public ValidatorSet GetValidatorSet()
    {
        var response = _graphQlHttpClient.SendQueryAsync<GetValidatorsResponseType>(
            new GraphQLRequest(
                @"query GetValidators($offsetBlockHash: ID!)
            {
                stateQuery
                {
                    validators(offsetBlockHash: $offsetBlockHash)
                    {
                        publicKey
                        power
                    }
                }
            }",
                operationName: "GetValidators",
                variables: new
                {
                    offsetBlockHash = _offset is { } hash
                        ? ByteUtil.Hex(hash.ByteArray)
                        : throw new NotSupportedException(),
                })).Result;

        return new ValidatorSet(response.Data.StateQuery.Validators
            .Select(x =>
                new Validator(new PublicKey(ByteUtil.ParseHex(x.PublicKey)), x.Power))
            .ToList());
    }

    private class GetAccountStateResponseType
    {
        public StateQueryWithAccountStateType StateQuery { get; set; }
    }

    private class StateQueryWithAccountStateType
    {
        public AccountStateType AccountState { get; set; }
    }

    public class AccountStateType
    {
        public string Address { get; set; }

        public string StateRootHash { get; set; }

        public string BlockHash { get; set; }
    }

    private class GetStatesResponseType
    {
        public StateQueryWithStatesType StateQuery { get; set; }
    }

    private class StateQueryWithStatesType
    {
        public byte[] States { get; set; }
    }

    private class GetBalanceResponseType
    {
        public StateQueryWithBalanceType StateQuery { get; set; }
    }

    private class StateQueryWithBalanceType
    {
        public FungibleAssetValueWithStringType Balance { get; set; }
    }

    private class FungibleAssetValueWithStringType
    {
        public string String { get; set; }
    }

    private class GetTotalSupplyResponseType
    {
        public StateQueryWithTotalSupplyType StateQuery { get; set; }
    }

    private class StateQueryWithTotalSupplyType
    {
        public FungibleAssetValueWithStringType TotalSupply { get; set; }
    }

    private class GetValidatorsResponseType
    {
        public StateQueryWithValidatorsType StateQuery { get; set; }
    }

    private class StateQueryWithValidatorsType
    {
        public ValidatorType[] Validators { get; set; }
    }

    private class ValidatorType
    {
        public string PublicKey { get; set; }
        public long Power { get; set; }
    }

    public bool Legacy { get; }
    public IAccount GetAccount(Address address)
    {
        throw new NotImplementedException();
    }

    
}
