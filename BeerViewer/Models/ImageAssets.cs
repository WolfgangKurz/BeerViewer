using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace BeerViewer.Models
{
	public class ImageAssets
	{
		internal static Image GetSlotIconImage(SlotItemIconType iconType)
		{
			switch (iconType)
			{
				case SlotItemIconType.MainCanonLight: return Properties.Resources.slotitem_MainCanonLight;
				case SlotItemIconType.MainCanonMedium: return Properties.Resources.slotitem_MainCanonMedium;
				case SlotItemIconType.MainCanonHeavy: return Properties.Resources.slotitem_MainCanonHeavy;
				case SlotItemIconType.SecondaryCanon: return Properties.Resources.slotitem_SecondaryCanon;

				case SlotItemIconType.Torpedo: return Properties.Resources.slotitem_Torpedo;

				case SlotItemIconType.Fighter: return Properties.Resources.slotitem_Fighter;
				case SlotItemIconType.DiveBomber: return Properties.Resources.slotitem_DiveBomber;
				case SlotItemIconType.TorpedoBomber: return Properties.Resources.slotitem_TorpedoBomber;
				case SlotItemIconType.ReconPlane: return Properties.Resources.slotitem_ReconPlane;
				case SlotItemIconType.ReconSeaplane: return Properties.Resources.slotitem_ReconSeaplane;

				case SlotItemIconType.Rader: return Properties.Resources.slotitem_Rader;
				case SlotItemIconType.AAShell: return Properties.Resources.slotitem_AAShell;
				case SlotItemIconType.APShell: return Properties.Resources.slotitem_APShell;
				case SlotItemIconType.DamageControl: return Properties.Resources.slotitem_DamageControl;

				case SlotItemIconType.AAGun: return Properties.Resources.slotitem_AAGun;
				case SlotItemIconType.HighAngleGun: return Properties.Resources.slotitem_HighAngleGun;

				case SlotItemIconType.ASW: return Properties.Resources.slotitem_ASW;
				case SlotItemIconType.Soner: return Properties.Resources.slotitem_Soner;

				case SlotItemIconType.EngineImprovement: return Properties.Resources.slotitem_EngineImprovement;
				case SlotItemIconType.LandingCraft: return Properties.Resources.slotitem_LandingCraft;

				case SlotItemIconType.Autogyro: return Properties.Resources.slotitem_Autogyro;
				case SlotItemIconType.ArtillerySpotter: return Properties.Resources.slotitem_ArtillerySpotter;

				case SlotItemIconType.AntiTorpedoBulge: return Properties.Resources.slotitem_AntiTorpedoBulge;

				case SlotItemIconType.Searchlight: return Properties.Resources.slotitem_Searchlight;
				case SlotItemIconType.DrumCanister: return Properties.Resources.slotitem_DrumCanister;
				case SlotItemIconType.Facility: return Properties.Resources.slotitem_Facility;
				case SlotItemIconType.Flare: return Properties.Resources.slotitem_Flare;

				case SlotItemIconType.FleetCommandFacility: return Properties.Resources.slotitem_FleetCommandFacility;
				case SlotItemIconType.MaintenancePersonnel: return Properties.Resources.slotitem_MaintenancePersonnel;

				case SlotItemIconType.AntiAircraftFireDirector: return Properties.Resources.slotitem_AntiAircraftFireDirector;
				case SlotItemIconType.RocketLauncher: return Properties.Resources.slotitem_RocketLauncher;

				case SlotItemIconType.SurfaceShipPersonnel: return Properties.Resources.slotitem_SurfaceShipPersonnel;
				case SlotItemIconType.FlyingBoat: return Properties.Resources.slotitem_FlyingBoat;

				case SlotItemIconType.CombatRations: return Properties.Resources.slotitem_CombatRations;
				case SlotItemIconType.OffshoreResupply: return Properties.Resources.slotitem_OffshoreResupply;

				case SlotItemIconType.AmphibiousLandingCraft: return Properties.Resources.slotitem_AmphibiousLandingCraft;

				case SlotItemIconType.LandBasedAttacker: return Properties.Resources.slotitem_LandBasedAttacker;
				case SlotItemIconType.LandBasedFighter: return Properties.Resources.slotitem_LandBasedFighter;

				case SlotItemIconType.JetPowerededBomber1: return Properties.Resources.slotitem_JetPowerededBomber1;
				case SlotItemIconType.JetPowerededBomber2: return Properties.Resources.slotitem_JetPowerededBomber2;

				case SlotItemIconType.TransportEquipment: return Properties.Resources.slotitem_TransportEquipment;
				case SlotItemIconType.SubmarineEquipment: return Properties.Resources.slotitem_SubmarineEquipment;
				case SlotItemIconType.SeaplaneFighter: return Properties.Resources.slotitem_SeaplaneFighter;

				default: return Properties.Resources.slotitem_Unknown;
			}
		}
		internal static Image GetProficiencyImage(int proficiency)
		{
			switch (proficiency)
			{
				case 1:
					return Properties.Resources.slotitem_proficiency_1;
				case 2:
					return Properties.Resources.slotitem_proficiency_2;
				case 3:
					return Properties.Resources.slotitem_proficiency_3;
				case 4:
					return Properties.Resources.slotitem_proficiency_4;
				case 5:
					return Properties.Resources.slotitem_proficiency_5;
				case 6:
					return Properties.Resources.slotitem_proficiency_6;
				case 7:
					return Properties.Resources.slotitem_proficiency_7;
				default:
					return null;
			}
		}
	}
}
