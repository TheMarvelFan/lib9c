﻿using System.Collections.Immutable;
using Libplanet;

namespace Nekoyume.BlockChain.Policy
{
    public static class PermanentBlockPolicySource
    {
        public const long AuthorizedMinersPolicyInterval = 2;

        public static readonly ImmutableHashSet<Address> AuthorizedMiners = new Address[]
        {
            new Address("82b857D3fE3Bd09d778B40f0a8430B711b3525ED"),
        }.ToImmutableHashSet();

        public static readonly ImmutableHashSet<Address> PermissionedMiners = new Address[]
        {
            new Address("211afcd0E152A61C92600D6a5a63Ca088a85Fbb1"),
            new Address("8a393e376d6Fd3b837314c7d4e249cc90a6B7B17"),
            new Address("a01EF08909Ab1F42f66173ee1e0736e72af18062"),
            new Address("9F2C5e93E8586D08a1045Ec58096f6422627C971"),
            new Address("8Bf91e5eF37Ee14Bc13F52143AF847Aa82399dc10"),
            new Address("916a980167bd625A208011956D27612939bFfE4c"),
            new Address("aEF27177D9D5a5266dA6F4cad61d795E53a1A36f"),
            new Address("eBda7706A48F02a98579048A01EB4Cb7Ce9bF721"),
            new Address("0f2Aa1617D7206afc4801C7ff5016C7162A30C33"),
            new Address("cD1D8ef767fb774c62Cd2cbda07Fe9935557342"),
            new Address("E97b955045589f95c14249c26954395DD48C92f10"),
            new Address("2f8a40FD4F133568dFC84bd1e850D243c9c04afd"),
            new Address("9a48b48fd57AC019B4868035D162B704f5Aa9421"),
            new Address("1012041FF2254f43d0a938aDF89c3f11867A2A59"),
            new Address("521a520e3d07D47425E3cc8385cA5893c7c3Cc31"),
            new Address("09Dcc04D5B6823261F220aB1A8e207d6626D3BC2"),
            new Address("41e77d761E832a0Decc8ea3Aa412dd99b84db119"),
            new Address("568F7c8E499d758A8384E2FB702879Ed7Efb328f"),
            new Address("A818C63ab70348619A3688ef06B0EdC49b693691"),
            new Address("D0043Fb67c33fBe14787e96573C5C4273A0ea4D5"),
            new Address("cB319Ba062123A4390b0D543a4188B126D9646C1"),
            new Address("d65c29a8fE89Bf8C4d2E3845e1001DDC95440F2D"),
            new Address("680Ede21da63D177375087Cb28603528486Ae680"),
            new Address("53765981c701e805EBf93f4f9984eE3b4960A4B3"),
            new Address("84D1a79fE16a98baca35cF0B84391A5CAdD92824"),
            new Address("D7B5F0c0b2d9b6804e9dcC6d855379838d3794EC"),
            new Address("AD1b4A3d126e2dFfF62c22a2ed83c497Ed0F619B"),
            new Address("D5A3bdf9c413bB5b8F244Ee29eaA2ed7cbEF5a91"),
            new Address("C0bA278CB8379683E66C28928fa0Aa8bfF3D95E7"),
            new Address("7c8042AA7c9B381C02A7a94a2bf7b3Fed179B645"),
            new Address("Ae451cB579a750184a76150E077dBC2bb507e40a"),
            new Address("aa046a288c2d7c94Fd64Ef18C6E9608fC046f3bB"),
            new Address("bE8d411c8721F775298e334247D819F51B9fB8cf"),
            new Address("eB873304312A9c5EFA7C3ee62A9AeA8275E22D1e"),
            new Address("EE59bbE0453a981F7D0D59bF4a0ab4cDDe46C58e"),
            new Address("79cc3c6976f8910be07c580e57b53ae47c534da3"),
            new Address("BFd6348e24D8140F774D168252255CB127D760Cd"),
            new Address("230d317FdC3c49485615826ffef4c294Fe3222D6"),
            new Address("5e64faccCE6305be5b9D1f81058E129768B346E7"),
            new Address("641D19A80C6D7FBe90A1447C6a4993489Fe61EdA"),
            new Address("a07a5c0F0eF19C544E4677c3f0Cb077FC304C73B"),
            new Address("b449743c204190c10c9be748bc01e71d985bb649"),
            new Address("1F8B5c69D6A349B2693671C7aEcf6ac5E717b7e9"),
            new Address("944b6a0392F8392A680c32FB5e739eEd73089F87"),
            new Address("a49d64c31A2594e8Fb452238C9a03beFD1119964"),
            new Address("3b2f76B0E34C2AdE71Ea04c1a765A732040237AF"),
            new Address("CacbfE3c2f45e2aA53605B95fE566793170CdA86"),
            new Address("c5988caa77cf3e0755121c11624f28c053054121"),
            new Address("dD9a6E3cA0c3F280260493Eb299b25Aa551a8bd3"),
            new Address("90F60d2773c627B4740b1e2113779Fd42eE541e3"),
            new Address("f392d97E4D1757070Fd5b4dB9cdB9bD024F2c00e"),
            new Address("37f301d53c48b5d3e5e2e7bd89f2c1aaa15cfe37"),
            new Address("35c7d6c584aD6cDFc67830ef02d1078E36D588d1"),
            new Address("cF3F29ce47318363E98A1D9B78442f71B6F57D33"),
            new Address("FBb4bE3595651AeA90ED5dA2Ba0216a391eeE5e6"),
            new Address("0e50540d1604AB21Fa167FA2e1d04C3635FcA4D10"),
            new Address("D3DC36514e7C2B1175e38d7C0dAA644D19F349E8"),
            new Address("493C16b25E6Be0735F85D78d3226bC3E1260eb60"),
            new Address("761928d99001824F8e19cd9d2B36F94CCd16BD87"),
            new Address("55762dE5aBF714d20e102AaBd89Bd08B3573d83"),
            new Address("AD4f4C11b867B0Ed558D47E126043293533b6d7e"),
            new Address("37B34EC47D2b1c2cE0A67CcBb71C001bbbEC0e8a"),
            new Address("ee9fCe20FC66F99E71A2AA2f33CB2426A02e13f5"),
            new Address("bB4e59279a526bb1E6B295b0C3fFeCF9BCb02b4D"),
            new Address("057B7843a6FCd8C4d24744b1E9F32A4a3b974729"),
            new Address("9a3b4a8fddeaafe1f27acc6fad5bad2bcd8d3916"),
            new Address("0268eB60f7efe8f5e3c4c6972D4a2072D47cb45C"),
            new Address("8B30A122D23D0c8AB5215880Dc0C2Fb0a4F4b8d4"),
            new Address("e6f030E3E9Df6628A8FE3707AE4DFD41664999cf"),
            new Address("9dbC7971C7f64B5F7B7FB261aE062fAfBAf99950"),
            new Address("818DAE737a355A1B72a15Eee220808191f9e1497"),
            new Address("00370420130BE479c909B5F2aCCFA427aB7a0B66"),
            new Address("eFDFe925B4DcD97814Ddd3C5947536cF5Aa8D85A"),
            new Address("9657561f4234a7c775E556468eD41f4de66c59AE"),
            new Address("bc7Cba0259304B3D952b700614Ffc6E40dfce9C3"),
            new Address("78c5D81B1e91f484173e65AcFE2C577F7471A4ce"),
            new Address("9d736E120ab3b3F1cdf650354429B7B9E414457B"),
            new Address("B571455dbe42D7F0C0AFE3D7618258Ec7cd06a05"),
            new Address("767D182aF477eF59d99683303eA2DF6B546dCA5d"),
            new Address("dDCe3c1416fbD7d0533145bE281FBF5efA90f001"),
            new Address("6B3f07E71d825Ef324138a50176C4497565232db"),
            new Address("e471834EC1581616974De76fFcC834e8D864e535"),
            new Address("8C4AbFA4cfEf11C5174Ed3bf1779F240B3ACC865"),
            new Address("74ea21446260bfe34a76e6a8b5eb6f72d83f2703"),
            new Address("c2abC5F77172C9e230c0c7C73e433f08F9952536"),
            new Address("66580ef464f060c4825d587871df7adf2c5c4168"),
            new Address("47C8448d46E573d9B5Bda827d0bD0A442d24753a"),
            new Address("E06B68C4DC6130bB6517C66368bBF8B4c6dE88Cb"),
            new Address("8Ef49a31d1b84C89A1606aAA1C8aAE4fc7eF8778"),
            new Address("7e2d38AA27D3529F66b922dbB14f1C9EE1A845C1"),
            new Address("CfaA743C6d3e957Dc7902057cD4b8217aA1BEBfc"),
            new Address("A3F6A5b53f85C5Ce7B0c6b73530217Cb5cF27035"),
            new Address("660A68eB86ec43D15163FB9633319C4520d1cF0a"),
            new Address("0C71ad15e4a5d1beAd393aCe3fA278e15783D933"),
            new Address("78BfF3051DFc698272382aB141D8682624f4F22f"),
            new Address("Dc4024cb60C9b5FbD2228f566803194606f82692"),
            new Address("1aB79b9ddAd78C9835b07078F48efBaf94354454"),
            new Address("c6999160E28F05B88A2e0efa077e23C1E51427A4"),
            new Address("E47E152F38109EcFbc73ff894114188496A315dB"),
            new Address("9EBD1b4F9DbB851BccEa0CFF32926d81eDf6De53"),
            new Address("C40D1C0C12202db456fCdDE20e7a865e6a581e22"),
            new Address("e08bab29dC3D40E0FFbd563FA3E1b80fB3afDa28"),
            new Address("55Cc273b11ce4eb44a679E381c4B7dFf6f89f7Ab"),
            new Address("a8Bd9B6339C914647701328E70c20C5F9A70802c"),
            new Address("C29d50994cDFCD9648f20Aa44acfBCFEc18E0b8f"),
            new Address("0A1a40a2acb68CF943C90bDc53F69bEC058997D3"),
            new Address("E4EBc565Ec9CC66D2a4aD81Ee8614678e60acd89"),
            new Address("C7FffFC194f81d2c5C5684C2cD7C731A1DDe133B"),
            new Address("06c47a46CC4F0Db32Ae4441033A5C72c4c95754b"),
            new Address("44E5abD51B460e546532F15897003abC09aDf8ea"),
            new Address("C14CBcAcC18b8ce707D9aF5fB489216e551aF0E7"),
            new Address("d936F927942AE879527b596BF87688f85F3A1e05"),
            new Address("D478d3A6cFe30C32F501e80D1d228C5d2881c4Cf"),
            new Address("6119a05c7932dD1b1c73E86900c68BC6Da7a66dE"),
            new Address("6D4072400a39b77e81928306ab5e57dE979dF595"),
            new Address("8f09189FeE5C73D397EDFe3bC408067a42B0FCad"),
            new Address("4Dc3769a59c62963c206786eeE65C13BB3b9160"),
            new Address("16B1974640544eF8a1E3e86530a1A61930bD5902"),
            new Address("9D3B9F6939c980ad52b6046425f6973BA7dEd998"),
            new Address("9aECbd10A0FAcc31671C69DCD46205268e513d69"),
            new Address("20ad91F7756C4612ab0F7286Dc7E674C80504CFD"),
            new Address("5d593dbFB6bC6869A0B3C2960B6799845183c513"),
            new Address("31F436CE8991Cc0e6796B49891FE474de9a0487D"),
            new Address("1D169ec5B341225516A5bb25DAc20F1d2EAe119A"),
            new Address("1b01292aB293410ba12E65E5c81A82d6395E7d0e"),
            new Address("7bF2D0b7BB590D51dda28d0052Ed09562eaD070c"),
            new Address("69783c52363e8b1008746f91f2d80E12C972B7e7"),
            new Address("7fe8268a17850179A17Fb2CDA0338e7d9549e0a3"),
            new Address("2dF52b3c1a8304483FE33A5093efb9bDcF0f1ea7"),
            new Address("6eF4DC9e7c8AafDBfF6058692d05345D3C4a6ccB"),
            new Address("15daE5ACb754251818bd362a2C9C5Bb01B75d844"),
            new Address("7d9877dB6ceE0e96F90f4715d55faEBa63E7371B"),
            new Address("c5c3F153424Cef46c678C84C23F8F17e265135E5"),
            new Address("f0763B75E38dd46008712C088E4EEd3333665e8F"),
            new Address("78010a2bc7f8551fb6267ae6bb26be489786ccbe"),
            new Address("F13D226c601595D4D2C7490c11EaaF35Ead941dA"),
            new Address("f2ecc5299A6E8e4c856DACf456a3b63f438294ae"),
            new Address("26aDf6a6170388620851E0A8dCb9D011747Df75f"),
            new Address("A41d26011cA1AEa631Bc25B8bf8B049066dcb8E1"),
            new Address("003FB7A11c6BaC61c12C25a2734898941aDF083e"),
            new Address("FaFe5D11A356Fe661B1deC1D17727e91e4Ca1b7c"),
            new Address("4529EbA8D2B11F782B8cCbb4CD0Ca5eB1eCeaa8D"),
            new Address("3d24CD1D6b6051f8B2B4Ba32DA8d2049831681F10"),
            new Address("318C5B0e5ba43F4F7b11c52a6B02C9d44E81936A"),
            new Address("bB793a08e5AE9a2392Cd30700EB8442FA1871A1f"),
            new Address("0054ea3a5288dF2f43Eb7b9651F46d2167CB47D7"),
            new Address("ada210AfeD6F0BFa33f1eCD2092Ce6b23Ee1A1E8"),
            new Address("40bF217C42483c16D57a9e0CEE7BbC84A8964Dc5"),
            new Address("Dcdffe41264A7f9e5E35fD120233A6673069F545"),
            new Address("79C7F57b4C2C036D4f7bb61140dd9cD0687A792B"),
            new Address("93150eC64033a3D14578761e4019634f00ccFC46"),
            new Address("977609EDc0dFc4dB692a28AAF43391ABc1B159b10"),
            new Address("DF644454f2A7B4b902c1D02D724FC9D3475317d3"),
            new Address("4884EA18f5b5d35EFb412786bBa16FB67b08b9CB"),
            new Address("14D0b29a27F0E5620104A6c6C2f4120f9177C21D"),
            new Address("9Bf0A825aE6b0F90E2E7eE211e1Cd63679fa58C2"),
            new Address("80209BE57c6b5E61702B71eBD26D504F6e4B27B8"),
            new Address("77a6321e7120985b7CcBC20115689D8e9595Fb4F"),
            new Address("7e4221b9F39C26C0f69ADa2fD101B7C5956B8dB5"),
            new Address("4db4ADb64FA9600713Ca5082cf3A469C926717eD"),
            new Address("415e147402BAc844aB0D84dBc6AEA1DAE9445B60"),
            new Address("b3569a79cd4498a8eb6f4d0cbd3057820156f671"),
            new Address("2eaEb937C5166BA14f6A25e4eb05767Eda42e1Ca"),
            new Address("9a5a8DaeAaa0386d7d7c82AcBD4d50fA7C69186D"),
            new Address("30619Da09908af4C0BEd7F1A52ff89CfA885aeF9"),
            new Address("1B7078cA7CAc120627A8c6eC564966318bB2866E"),
            new Address("6573572ABeb9B27a6511E77fEEE67C03d7434066"),
            new Address("6EAeEb72F1319aB9764AF716Cd8102175816ce4c"),
            new Address("6D7871B01b1A5039A4e5E6DaD3d4D1a7aBBCa76B"),
            new Address("b20732BEe6f9F61b568920039B828C4de9a6Fb7A"),
            new Address("b5Ef23b246c944f3C7308597e3C9426C8Adc7cd5"),
            new Address("b4E8476BF840f9371269b164587DD5fb85AC74A6"),
            new Address("31540d2FED75719567B5177b9e98a4644F4916F3"),
            new Address("a84D1b09d58394A1C2BfE5Aaf4987eDA9E28Fc14"),
            new Address("DcB9EB5A77b761e46DbE8d73C375782b082fB4c3"),
            new Address("8Ef1c48e5126001e13244E6B56c66EC868A0BE1A"),
            new Address("bB279dE1a8EF6B7aA8407033A0D8cC0FFf6d4BB5"),
            new Address("936a7bd0B33Faaf1bB9Af84eFCADA8Bf42AEF72d"),
            new Address("A41d26011cA1AEa631Bc25B8bf8B049066dcb8E1"),
            new Address("E93d31A3B0c937644AF5dD4a302a0C211a1394E10"),
            new Address("590c887BDac8d957Ca5d3c1770489Cf2aFBd868E"),
        }.ToImmutableHashSet();
    }
}
