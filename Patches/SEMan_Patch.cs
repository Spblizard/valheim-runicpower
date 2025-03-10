﻿using HarmonyLib;
using UnityEngine;

namespace RunicPower.Patches {

	[HarmonyPatch(typeof(SEMan), "OnDamaged")]
	public static class Character_OnDamaged_Patch {
		static void Prefix(SEMan __instance, ref HitData hit, Character attacker) {
			if (hit == null) return;
			if (attacker == null) return;

			var prSteal = attacker.ExtendedCharacter(false)?.runicLifeSteal ?? 0;
			if (prSteal <= 0) return;

			var totalf = hit.GetTotalDamage();
			var back = totalf * prSteal / 100f;
			attacker.Heal(back);
		}
	}

	[HarmonyPatch(typeof(SEMan), "AddStatusEffect", typeof(StatusEffect), typeof(bool), typeof(int), typeof(float))]
	public static class SEMan_AddStatusEffect_Patch {
		static void Postfix(SEMan __instance, StatusEffect statusEffect, bool resetTime, int itemLevel, float skillLevel,  ref StatusEffect __result) {
			var rune = SEMan_Prototype.GetTempRune(statusEffect);
			if (rune == null) return;

			__result.SetRune(rune);
			__instance.m_character?.ExtendedCharacter(true)?.AddRune(rune);

			SEMan_Prototype.UnsetTemp();
		}
	}

	[HarmonyPatch(typeof(SEMan), "ModifyStealth")]
	public static class SEMan_ModifyStealth_Patch {
		public static void Postfix(SEMan __instance, float baseStealth, ref float stealth) {
			var runic = __instance.m_character?.ExtendedCharacter(true)?.runicStealth ?? 0;
			if (runic != 0) {
				var bf = stealth;
				stealth -= runic / 100f;
			}
		}
	}
}
