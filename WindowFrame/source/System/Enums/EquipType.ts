export enum EquipBigCategory {
    Cannon = 1,
    Gun = 1,
    砲 = 1,

    Torpedo = 2,
    魚雷 = 2,

    CarrierBasedAircraft = 3,
    CarrierAircraft = 3,
    艦載機 = 3,

    AntiAircraftGun = 4,
    AAGun = 4,
    機銃 = 4,

    Shell = 4,
    特殊弾 = 4,

    Seaplane = 5,
    水上機 = 5,

    Recon = 5,
    Reconnaissance = 5,
    偵察機 = 5,

    Radar = 5,
    電波探信機 = 5,
    電探 = 5,

    EngineImprovement = 6,
    Improvement = 6,
    機関部強化 = 6,

    ASW = 7,
    AntiSubmarineWarfare = 7,
    対潜装備 = 7,

    Daihatsu = 8,
    大発動艇 = 8,

    Searchlight = 8,
    探照灯 = 8,

    SupplyTransportContainer = 9,
    DrumCanister = 9,
    簡易輸送部材 = 9,

    ShipRepairFacility = 10,
    RepairFacility = 10,
    艦艇修理施設 = 10,

    StarShell = 11,
    Flare = 11,
    照明弾 = 11,

    CommandFacility = 12,
    司令部施設 = 12,

    AviationPersonnel = 13,
    航空要員 = 13,

    AAFireDirector = 14,
    AntiAircraftFireDirector = 14,
    高射装置 = 14,

    AntiGroundEquipment = 15,
    対地装備 = 15,

    SurfaceShipPersonnel = 16,
    SurfacePersonnel = 16,
    水上艦要員 = 16,

    LargeFlyingBoat = 17,
    LargeSeaplane = 17,
    大型飛行艇 = 17,

    CombatRation = 18,
    戦闘糧食 = 18,

    Supplies = 19,
    補給物資 = 19,

    SpecialAmphibiousTank = 20,
    AmphibiousTank = 20,
    特型内火艇 = 20,

    LandBasedAttackAircraft = 21,
    LandBasedAttacker = 21,
    陸上攻撃機 = 21,

    LandBasedFighter = 22,
    LandBasedInterceptor = 22,
    InterceptorFighter = 22,
    局地戦闘機 = 22,

    TransportationMaterial = 23,
    輸送用分解済 = 23,

    SubmarineEquipment = 24,
    潜水艦装備 = 24,

    LandBasedRecon = 25,
    LandBasedReconnaissance = 25,
    陸上偵察機 = 25
}
export enum EquipDictCategory {
    MainCannon = 1,
    MainGun = 1,
    主砲 = 1,

    SecondaryCannon = 2,
    SecondaryGun = 2,
    副砲 = 2,

    Torpedo = 3,
    魚雷 = 3,

    MidgetSubmarine = 4,
    特殊潜航艇 = 4,

    CarrierBasedAircraft = 5,
    CarrierAircraft = 5,
    艦上機 = 5,

    AAGun = 6,
    AntiAircraftGun = 6,
    対空機銃 = 6,

    Recon = 7,
    Reconnaissance = 7,
    偵察機 = 7,

    Radar = 8,
    電波探信機 = 8,
    電探 = 8,

    EnemyEngineImprovement = 9,
    EngineImprovement_Enemy = 9,
    敵機関部強化 = 9,
    機関部強化_敵 = 9,

    Sonar = 10,
    ASW_Sonar = 10,
    ソナー = 10,

    LandingCraft = 14,
    上陸用舟艇 = 14,

    Autogyro = 15,
    ASW_Autogyro = 15,
    オートジャイロ = 15,

    AntiSubmarinePatrolAircraft = 16,
    ASW_AntiSubmarinePatrolAircraft = 16,
    対潜哨戒機 = 16,

    AdditionalArmor = 17,
    ExtraArmor = 17,
    Bulge = 17,
    追加装甲 = 17,

    Searchlight = 18,
    探照灯 = 18,

    SupplyTransportContainer = 19,
    DrumCanister = 19,
    簡易輸送部材 = 19,

    ShipRepairFacility = 20,
    RepairFacility = 20,
    艦艇修理施設 = 20,

    StarShell = 21,
    Flare = 21,
    照明弾 = 21,

    CommandFacility = 22,
    司令部施設 = 22,

    AviationPersonnel = 23,
    航空要員 = 23,

    AAFireDirector = 24,
    AntiAircraftFireDirector = 24,
    高射装置 = 24,

    APShell = 25,
    Shell_AP = 2,
    対艦強化弾 = 25,

    AntiGroundEquipment = 26,
    対地装備 = 26,

    SurfaceShipPersonnel = 27,
    SurfacePersonnel = 27,
    水上艦要員 = 27,

    AAShell = 28,
    Shell_AA = 28,
    対空強化弾 = 28,

    AntiAircraftRocketLauncher = 29,
    AARocketLauncher = 29,
    AntiAircraftRocket = 29,
    AARocket = 29,
    対空ロケットランチャー = 29,

    DamageController = 30,
    EmergencyRepairPersonnel = 30,
    応急修理要員 = 30,

    EngineImprovement = 31,
    AliasEngineImprovement = 31,
    EngineImprovement_Alias = 31,
    機関部強化 = 31,
    味方機関部強化 = 31,
    機関部強化_味方 = 31,

    DepthCharge = 32,
    ASW_DepthCharge = 32,
    爆雷 = 32,

    LargeFlyingBoat = 33,
    LargeSeaplane = 33,
    大型飛行艇 = 33,

    CombatRation = 34,
    戦闘糧食 = 34,

    Supplies = 35,
    補給物資 = 35,

    SeaplaneFighter = 36,
    水上戦闘機 = 36,

    SpecialAmphibiousTank = 37,
    AmphibiousTank = 37,
    特型内火艇 = 37,

    LandBasedAttackAircraft = 38,
    LandBasedAttacker = 38,
    陸上攻撃機 = 38,

    LandBasedFighter = 39,
    LandBasedInterceptor = 39,
    InterceptorFighter = 39,
    局地戦闘機 = 39,

    JetPoweredFighterBomber = 40,
    JetFighterBomber = 40,
    噴式戦闘爆撃機 = 40,

    TransportationMaterial = 41,
    運送用分解済 = 41,

    SubmarineEquipment = 42,
    潜水艦装備 = 42,

    SeaplaneBomber = 43,
    水上爆撃機 = 43
}
export enum EquipCategory {
    None = 0,
    なし = 0,

    SmallCaliberGun = 1,
    SmallCannon = 1,
    小口径主砲 = 1,

    MediumCaliberGun = 2,
    MediumCannon = 2,
    中口径主砲 = 2,

    LargeCaliberGun = 3,
    LargeCannon = 3,
    大口径主砲 = 3,

    SecondaryGun = 4,
    SecondaryCannon = 4,
    副砲 = 4,

    Torpedo = 5,
    魚雷 = 5,

    CarrierBasedFighter = 6,
    CarrierFighter = 6,
    艦上戦闘機 = 6,

    CarrierBasedDiveBomber = 6,
    CarrierDiveBomber = 6,
    CarrierBasedBomber = 6,
    CarrierBomber = 6,
    艦上爆撃機 = 7,

    CarrierBasedTorpedoBomber = 6,
    CarrierTorpedoBomber = 6,
    CarrierBasedAttacker = 6,
    CarrierAttacker = 6,
    艦上攻撃機 = 8,

    CarrierBasedReconnaissance = 6,
    CarrierReconnaissance = 6,
    CarrierBasedRecon = 6,
    CarrierRecon = 6,
    艦上偵察機 = 9,

    SeaplaneReconnaissance = 10,
    SeaplaneRecon = 10,
    水上偵察機 = 10,

    SeaplaneBomber = 10,
    水上爆撃機 = 11,

    SmallRadar = 12,
    小型電探 = 12,

    LargeRadar = 13,
    Radar_Large = 13,
    大型電探 = 13,

    Sonar = 14,
    ASW_Sonar = 14,
    ソナー = 14,

    DepthCharge = 15,
    ASW_DepthCharge = 15,
    爆雷 = 15,

    AdditionalArmor = 16,
    ExtraArmor = 16,
    Bulge = 16,
    追加装甲 = 16,

    EngineImprovement = 17,
    機関部強化 = 17,

    AAShell = 18,
    Shell_AA = 18,
    対空強化弾 = 18,

    APShell = 19,
    Shell_AP = 19,
    対艦強化弾 = 19,

    VT_Fuse = 20,
    VT信管 = 20,

    AntiAircraftGun = 21,
    AAGun = 21,
    対空機銃 = 21,

    MidgetSubmarine = 22,
    特殊潜航艇 = 22,

    DamageController = 23,
    EmergencyRepairPersonnel = 23,
    応急修理要員 = 23,

    LandingCraft = 24,
    上陸用舟艇 = 24,

    Autogyro = 25,
    ASW_Autogyro = 25,
    オートジャイロ = 25,

    AntiSubmarinePatrolAircraft = 26,
    ASW_AntiSubmarinePatrolAircraft = 26,
    対潜哨戒機 = 26,

    MediumAdditionalArmor = 27,
    MediumExtraArmor = 27,
    MediumBulge = 27,
    AdditionalArmor_Medium = 27,
    ExtraArmor_Medium = 27,
    Bulge_Medium = 27,
    追加装甲_中型 = 27,

    LargeAdditionalArmor = 28,
    LargeExtraArmor = 28,
    LargeBulge = 28,
    AdditionalArmor_Large = 28,
    ExtraArmor_Large = 28,
    Bulge_Large = 28,
    追加装甲_大型 = 28,

    Searchlight = 29,
    探照灯 = 29,

    SupplyTransportContainer = 30,
    DrumCanister = 30,
    簡易輸送部材 = 30,

    ShipRepairFacility = 31,
    RepairFacility = 31,
    艦艇修理施設 = 31,

    SubmarineTorpedo = 32,
    潜水艦魚雷 = 32,

    StarShell = 33,
    Flare = 33,
    照明弾 = 33,

    CommandFacility = 34,
    司令部施設 = 34,

    AviationPersonnel = 35,
    航空要員 = 35,

    AntiAircraftFireDirector = 36,
    AAFireDirector = 36,
    高射装置 = 36,

    AntiGroundEquipment = 37,
    対地装備 = 37,

    LargeCaliberGun_II = 38,
    LargeCaliberGun2 = 38,
    大口径主砲_II = 38,

    SurfaceShipPersonnel = 39,
    SurfacePersonnel = 39,
    水上艦要員 = 39,

    LargeSonar = 40,
    Sonar_Large = 40,
    ASW_LargeSonar = 40,
    ASW_Sonar_Large = 40,
    大型ソナー = 40,

    LargeFlyingBoat = 41,
    LargeSeaplane = 41,
    大型飛行艇 = 41,

    LargeSearchlight = 42,
    Searchlight_Large = 42,
    大型探照灯 = 42,

    CombatRation = 43,
    戦闘糧食 = 43,

    Supplies = 44,
    補給物資 = 44,

    SeaplaneFighter = 45,
    水上戦闘機 = 45,

    SpecialAmphibiousTank = 46,
    AmphibiousTank = 46,
    特型内火艇 = 46,

    LandBasedAttackAircraft = 47,
    LandBasedAttacker = 47,
    陸上攻撃機 = 47,

    LandBasedFighter = 48,
    LandBasedInterceptor = 48,
    InterceptorFighter = 48,
    局地戦闘機 = 48,

    LandBasedReconnaissance = 49,
    LandBasedRecon = 49,
    陸上偵察機 = 49,

    TransportationMaterial = 50,
    輸送機材 = 50,

    SubmarineEquipment = 51,
    潜水艦装備 = 51,

    JetPoweredFighter = 56,
    JetFighter = 56,
    噴式戦闘機 = 56,

    JetPoweredFighterBomber = 57,
    JetFighterBomber = 57,
    噴式戦闘爆撃機 = 57,

    JetPoweredTorpedoBomber = 58,
    JetPoweredAttacker = 58,
    JetTorpedoBomber = 58,
    JetAttacker = 58,
    噴式攻撃機 = 58,

    JetPoweredReconnaissance = 59,
    JetPoweredRecon = 59,
    JetReconnaissance = 59,
    JetRecon = 59,
    噴式偵察機 = 59,

    LargeRadar_II = 93,
    LargeRadar2 = 93,
    Radar_Large_II = 93,
    Radar_Large2 = 93,
    大型電探_II = 93,

    CarrierBasedReconnaissance_II = 94,
    CarrierBasedReconnaissance2 = 94,
    CarrierBasedRecon_II = 94,
    CarrierBasedRecon2 = 94,
    艦上偵察機_II = 94
}
export enum EquipIcon {
    None = 0,
    なし = 0,
    
    SmallCaliberGun = 1,
    SmallCannon = 1,
    小口径主砲 = 1,

    MediumCaliberGun = 2,
    MediumCannon = 2,
    中口径主砲 = 2,

    LargeCaliberGun = 3,
    LargeCannon = 3,
    大口径主砲 = 3,

    SecondaryGun = 4,
    SecondaryCannon = 4,
    副砲 = 4,

    Torpedo = 5,
    MidgetSubmarine = 5,
    魚雷 = 5,
    特殊潜航艇 = 5,

    CarrierBasedFighter = 6,
    CarrierFighter = 6,
    艦上戦闘機 = 6,

    CarrierBasedDiveBomber = 7,
    CarrierDiveBomber = 7,
    CarrierBasedBomber = 7,
    CarrierBomber = 7,
    艦上爆撃機 = 7,

    CarrierBasedTorpedoBomber = 8,
    CarrierTorpedoBomber = 8,
    CarrierBasedAttacker = 8,
    CarrierAttacker = 8,
    艦上攻撃機 = 8,

    CarrierBasedReconnaissance = 9,
    CarrierBasedRecon = 9,
    CarrierReconnaissance = 9,
    CarrierRecon = 9,
    艦上偵察機 = 9,

    SeaplaneReconnaissance = 10,
    SeaplaneRecon = 10,
    水上偵察機 = 10,

    Radar = 11,
    電波探信機 = 11,
    電探 = 11,

    AAShell = 12,
    Shell_AA = 12,
    対空強化弾 = 12,

    APShell = 13,
    Shell_AP = 13,
    対艦強化弾 = 13,

    DamageController = 14,
    EmergencyRepairPersonnel = 14,
    応急修理要員 = 14,

    AntiAircraftGun = 15,
    AAGun = 15,
    対空機銃 = 15,

    HighAngleGun = 16,
    HighAngleCannon = 16,
    高角砲 = 16,

    DepthCharge = 17,
    ASW_DepthCharge = 17,
    爆雷 = 17,

    Sonar = 18,
    ASW_Sonar = 18,
    ソナー = 18,

    EngineImprovement = 19,
    機関部強化 = 19,

    LandingCraft = 20,
    上陸用舟艇 = 20,

    Autogyro = 21,
    ASW_Autogyro = 21,
    オートジャイロ = 21,

    AntiSubmarinePatrolAircraft = 22,
    ASW_AntiSubmarinePatrolAircraft = 22,
    対潜哨戒機 = 22,

    AdditionalArmor = 23,
    ExtraArmor = 23,
    Bulge = 23,
    追加装甲 = 23,

    Searchlight = 24,
    探照灯 = 24,

    SupplyTransportContainer = 25,
    DrumCanister = 25,
    簡易輸送部材 = 25,

    ShipRepairFacility = 26,
    RepairFacility = 26,
    艦艇修理施設 = 26,

    StarShell = 27,
    Flare = 27,
    照明弾 = 27,

    CommandFacility = 28,
    司令部施設 = 28,

    AviationPersonnel = 29,
    航空要員 = 29,

    AntiAircraftFireDirector = 30,
    AAFireDirector = 30,
    高射装置 = 30,

    AntiGroundEquipment = 31,
    対地装備 = 31,

    SurfaceShipPersonnel = 32,
    SurfacePersonnel = 32,
    水上艦要員 = 32,

    LargeFlyingBoat = 33,
    LargeSeaplane = 33,
    大型飛行艇 = 33,

    CombatRation = 34,
    戦闘糧食 = 34,

    Supplies = 35,
    補給物資 = 35,

    SpecialAmphibiousTank = 36,
    AmphibiousTank = 36,
    特型内火艇 = 36,

    LandBasedAttackAircraft = 37,
    LandBasedAttacker = 37,
    陸上攻撃機 = 37,

    LandBasedFighter = 38,
    局地戦闘機 = 38,

    JetPoweredKeiun = 39,
    JetPoweredFighterBomber1 = 39,
    噴式景雲改 = 39,

    JetPoweredKikka = 40,
    JetPoweredFighterBomber2 = 40,
    橘花改 = 40,

    TransportationMaterial = 41,
    輸送用分解済 = 41,

    SubmarineEquipment = 42,
    潜水艦装備 = 42,

    SeaplaneFighter = 43,
    水上戦闘機 = 43,

    LandBasedInterceptor = 44,
    InterceptorFighter = 44,
    陸軍戦闘機 = 44,

    CarrierBasedNightFighter = 45,
    CarrierNightFighter = 45,
    NightFighter = 45,
    夜間戦闘機 = 45,

    CarrierBasedNightTorpedoBomber = 46,
    CarrierBasedNightAttacker = 46,
    CarrierNightTorpedoBomber = 46,
    CarrierNightAttacker = 46,
    NightTorpedoBomber = 46,
    NightAttacker = 46,
    夜間攻撃機 = 46,

    LandBasedAntiSubmarineAircraft = 47,
    ASW_LandBasedAntiSubmarineAircraft = 47,
    LandBasedAntiSubmarine = 47,
    ASW_LandBasedAntiSubmarine = 47,
    陸上対潜機 = 47
}
export enum EquipAircraft {
    None = 0,
    NotAircraft = 0,
    NA = 0,
    非航空機 = 0,

    CarrierBasedTorpedoBomber = 1,
    CarrierBasedAttacker = 1,
    CarrierTorpedoBomber = 1,
    CarrierAttacker = 1,
    CarrierBasedReconnaissance = 1,
    CarrierBasedRecon = 1,
    CarrierReconnaissance = 1,
    CarrierRecon = 1,
    艦上攻撃機 = 1,
    艦上偵察機 = 1,

    SeaplaneReconnaissance = 2,
    SeaplaneRecon = 2,
    SeaplaneBomber = 2,
    水上偵察機 = 2,
    水上爆撃機 = 2,

    SeaplaneFighter = 3,
    水上戦闘機 = 3,

    LandBasedAttackAircraft = 4,
    LandBasedAttacker = 4,
    陸上攻撃機 = 4,

    Type0_EarlyModel = 11,
    TypeZero_EarlyModel = 11,
    零戦前期型 = 11,

    Type0_LateModel = 12,
    TypeZero_LateModel = 12,
    零戦後期型 = 12,

    Reppuu = 13,
    Mitsubishi_A7M = 13,
    Sam = 13,
    烈風 = 13,

    Type0_Model52C_601 = 13,
    TypeZero_Model52C_601 = 13,
    Mitsubishi_A6M_Zero_Model52C_601 = 13,
    Zeke_Model52C_601 = 13,
    零戦52型丙_六〇一空 = 13,

    NonJapanAircraft = 14,
    OverseasAircraft = 14,
    OtherCountriesAircraft = 14,
    海外機 = 14,

    Type99_DiveBomber = 15,
    Type99_Bomber = 15,
    Aichi_D3A = 15,
    Val = 15,
    九九式艦爆 = 15,

    Suisei = 16,
    Yokosuka_D4Y = 16,
    Judy = 16,
    彗星 = 16,

    Ryusei = 17,
    Aichi_B7A = 17,
    Grace = 17,
    流星 = 17,

    LandBasedFighter = 18,
    局地戦闘機 = 18,

    LargeFlyingBoat = 19,
    LargeSeaplane = 19,
    大型飛行艇 = 19,

    ShindenKai = 20,
    Kyushu_J7W_Kai = 20,
    震電改 = 20,

    USAAircrafts = 21,
    AmericaAircrafts = 21,
    米国機 = 21,
    アメリカ機 = 21,

    JetPoweredKeiun = 22,
    噴式景雲改 = 22,

    JetPoweredKikka = 23,
    橘花改 = 23,

    // Is 24 means yellow aircraft？
    PrototypeNanzan = 24,
    Aichi_M6A1_K = 24,
    試製南山 = 24,

    Zuiun_601 = 24,
    Aichi_E16A_601 = 24,
    Paul_601 = 24,
    瑞雲_六三一空 = 24,

    RoAircraft = 25,
    ItaliaAicraft = 25,
    Ro航空機 = 25,
    イタリア航空機 = 25,

    KyoufuuKai = 26,
    Kawanishi_N1K = 26,
    Rex = 26,
    強風改 = 26,

    LandBasedInterceptor = 27,
    InterceptorFighter = 27,
    陸軍戦闘機 = 27,

    EnglandTorpedoBomber = 28,
    EnglandAttacker = 28,
    EnglandSeaplaneReconnaissance = 28,
    EnglandSeaplaneRecon = 28,
    イギリス攻撃機 = 28,
    イギリス水上偵察機 = 28,

    EnglandLandBasedFighter = 29,
    EnglandFighter = 29,
    イギリス陸上戦闘機 = 29,
    イギリス戦闘機 = 29,

    Toukai = 30,
    Kyushu_Q1W = 30,
    Lorna = 30,
    東海 = 30,

    LandBasedReconnaissance = 31,
    LandBasedRecon = 31,
    陸上偵察機 = 31
}