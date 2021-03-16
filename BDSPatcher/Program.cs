using System;
using System.Collections.Generic;
using System.Linq;
using Mutagen.Bethesda;
using SSEForms = Mutagen.Bethesda.FormKeys.SkyrimSE;
using Mutagen.Bethesda.Synthesis;
using Mutagen.Bethesda.Skyrim;
using System.Threading.Tasks;

namespace BDSPatcher
{
    public class Program
    {
        public static async Task<int> Main(string[] args)
        {
            return await SynthesisPipeline.Instance
                .AddPatch<ISkyrimMod, ISkyrimModGetter>(RunPatch)
                .Run(args, new RunPreferences()
                {
                    ActionsForEmptyArgs = new RunDefaultPatcher()
                    {
                        IdentifyingModKey = "BDSPatcher.esp",
                        TargetRelease = GameRelease.SkyrimSE,
                    }
                });
        }

        private static readonly ModKey SkyrimModKey = ModKey.FromNameAndExtension("Skyrim.esm");
        private static readonly ModKey USSEPModKey = ModKey.FromNameAndExtension("Unofficial Skyrim Special Edition Patch.esp");
        private static readonly ModKey BDSModKey = ModKey.FromNameAndExtension("Better Dynamic Snow.esp");

        private static IReadOnlyDictionary<FormKey, IMaterialObjectGetter> MaterialMapping(ISkyrimModGetter bds)
        {
            var bdsMaterials = bds.MaterialObjects;
            if (bdsMaterials == null)
            {
                throw new ArgumentException("Unable to get Better Dynamic Snow MATO objects");
            }
            Dictionary<FormKey, IMaterialObjectGetter> mappings = new Dictionary<FormKey, IMaterialObjectGetter>();
            // SnowMaterialObjectNoise1P
            IMaterialObjectGetter? material = GroupExt.TryGetValue(bdsMaterials, new FormKey(BDSModKey, 0x39f5));
            if (material != null)
            {
                mappings.Add(SSEForms.Skyrim.MaterialObject.SnowMaterialObjectNoise1P, material);
            }
            // SnowMaterialObject1P
            material = GroupExt.TryGetValue(bdsMaterials, new FormKey(BDSModKey, 0x1305));
            if (material != null)
            {
                mappings.Add(SSEForms.Skyrim.MaterialObject.SnowMaterialObject1P, material);
            }
            // SnowMaterialMountain1P
            material = GroupExt.TryGetValue(bdsMaterials, new FormKey(BDSModKey, 0x1868));
            if (material != null)
            {
                mappings.Add(SSEForms.Skyrim.MaterialObject.SnowMaterialMountain1P, material);
            }
            // SnowMaterialObjectCustom03_1P
            material = GroupExt.TryGetValue(bdsMaterials, new FormKey(BDSModKey, 0x39f4));
            if (material != null)
            {
                mappings.Add(SSEForms.Skyrim.MaterialObject.SnowMaterialObjectCustom03_1P, material);
            }
            // SnowMaterialMountainTrimLight1P
            // xEdit Script does not check the Form ID and so would not map this
            material = GroupExt.TryGetValue(bdsMaterials, new FormKey(BDSModKey, 0x2372));
            if (material != null)
            {
                mappings.Add(SSEForms.Skyrim.MaterialObject.SnowMaterialMountainTrimLight1P, material);
            }
            // SnowMaterialWinterhold
            material = GroupExt.TryGetValue(bdsMaterials, new FormKey(BDSModKey, 0x2373));
            if (material != null)
            {
                mappings.Add(SSEForms.Skyrim.MaterialObject.SnowMaterialWinterhold, material);
            }
            // SnowMaterialMarkarthCliffs
            material = GroupExt.TryGetValue(bdsMaterials, new FormKey(BDSModKey, 0x1304));
            if (material != null)
            {
                mappings.Add(SSEForms.Skyrim.MaterialObject.SnowMaterialMarkarthCliffs, material);
            }
            // SnowMaterialObjectLight1P
            material = GroupExt.TryGetValue(bdsMaterials, new FormKey(BDSModKey, 0x44b8));
            if (material != null)
            {
                mappings.Add(SSEForms.Skyrim.MaterialObject.SnowMaterialObjectLight1P, material);
            }
            // SnowMaterialFarm
            material = GroupExt.TryGetValue(bdsMaterials, new FormKey(BDSModKey, 0x12ef));
            if (material != null)
            {
                mappings.Add(SSEForms.Skyrim.MaterialObject.SnowMaterialFarm, material);
            }
            // SnowMaterialStockade1P
            material = GroupExt.TryGetValue(bdsMaterials, new FormKey(BDSModKey, 0x12f0));
            if (material != null)
            {
                mappings.Add(SSEForms.Skyrim.MaterialObject.SnowMaterialStockade1P, material);
            }
            // SnowMaterialMountainTrim1P
            material = GroupExt.TryGetValue(bdsMaterials, new FormKey(BDSModKey, 0x12f1));
            if (material != null)
            {
                mappings.Add(SSEForms.Skyrim.MaterialObject.SnowMaterialMountainTrim1P, material);
            }
            // SnowMaterialRoadLight1P
            material = GroupExt.TryGetValue(bdsMaterials, new FormKey(BDSModKey, 0x2374));
            if (material != null)
            {
                mappings.Add(SSEForms.Skyrim.MaterialObject.SnowMaterialRoadLight1P, material);
            }
            // SnowMaterialObjectCustom01_1P
            material = GroupExt.TryGetValue(bdsMaterials, new FormKey(BDSModKey, 0x39f2));
            if (material != null)
            {
                mappings.Add(SSEForms.Skyrim.MaterialObject.SnowMaterialObjectCustom01_1P, material);
            }
            // SnowMaterialMountainLight1P
            material = GroupExt.TryGetValue(bdsMaterials, new FormKey(BDSModKey, 0x2361));
            if (material != null)
            {
                mappings.Add(SSEForms.Skyrim.MaterialObject.SnowMaterialMountainLight1P, material);
            }
            // SnowMaterialObjectCustom02_1P
            material = GroupExt.TryGetValue(bdsMaterials, new FormKey(BDSModKey, 0x39f3));
            if (material != null)
            {
                mappings.Add(SSEForms.Skyrim.MaterialObject.SnowMaterialObjectCustom02_1P, material);
            }

            return mappings;
        }
        public static void RunPatch(IPatcherState<ISkyrimMod, ISkyrimModGetter> state)
        {
            if (!state.LoadOrder.TryGetValue(BDSModKey, out IModListing<ISkyrimModGetter>? bdsMod) || bdsMod.Mod == null)
            {
                throw new ArgumentException("Unable to get Better Dynamic Snow.esp plugin");
            }
            IReadOnlyDictionary<FormKey, IMaterialObjectGetter> materialMapping = MaterialMapping(bdsMod.Mod);
            var skipMods = ImplicitListings.GetListings(state.PatchMod.GameRelease).ToHashSet();
            skipMods.Add(USSEPModKey);

            Console.WriteLine("{0} STAT", state.LoadOrder.PriorityOrder.WinningOverrides<IStaticGetter>().Count<IStaticGetter>());
            // skip STATs from excluded mods, provided BDS last patched the material
            foreach (var target in state.LoadOrder.PriorityOrder.WinningOverrides<IStaticGetter>().
                Where(stat => !skipMods.Contains(stat.FormKey.ModKey) || stat.Material.FormKey.ModKey != BDSModKey))
            {

                if (!materialMapping.TryGetValue(target.Material.FormKey, out IMaterialObjectGetter? mapped) || mapped == null)
                {
                    continue;
                }
                var matName = target.Material;
                Console.WriteLine("MATO {0:X8} mapped to BDS {1:X8} in STAT {2}:{3}/{4:X8} with flags {5}",
                    matName.FormKey.ID, mapped.FormKey.ID, target.FormKey.ModKey.FileName,
                    target.EditorID, target.FormKey.ID, target.Flags);

                var newStatic = state.PatchMod.Statics.GetOrAddAsOverride(target);
                newStatic.Material = new FormLink<IMaterialObjectGetter>(mapped.FormKey);
                //// TODO fix up SLAWF special case, data loss somehow
                ///// https://www.nexusmods.com/skyrimspecialedition/mods/26138
                //if (newStatic.EditorID == "RockCliff08_HeavySN_lawf")
                //{
                //    newStatic.Flags = Static.Flag.ConsideredSnow;
                //}
                //else
                //{
                //    newStatic.Flags = target.Flags;
                //}
            }
        }
    }
}
