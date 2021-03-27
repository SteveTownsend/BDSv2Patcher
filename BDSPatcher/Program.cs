using System;
using System.Collections.Generic;
using System.Linq;
using Mutagen.Bethesda;
using SSEForms = Mutagen.Bethesda.FormKeys.SkyrimSE;
using Mutagen.Bethesda.Synthesis;
using Mutagen.Bethesda.Skyrim;
using System.Threading.Tasks;
using Noggog;

namespace BDSPatcher
{
    public class Program
    {
        public static async Task<int> Main(string[] args)
        {
            return await SynthesisPipeline.Instance
                .SetTypicalOpen(GameRelease.SkyrimSE, "BDSPatcher.esp")
                .AddPatch<ISkyrimMod, ISkyrimModGetter>(RunPatch)
                .Run(args);
        }

        private static readonly ModKey USSEPModKey = ModKey.FromNameAndExtension("Unofficial Skyrim Special Edition Patch.esp");
        private static readonly ModKey BDSModKey = ModKey.FromNameAndExtension("Better Dynamic Snow.esp");

        private static IReadOnlyDictionary<IFormLinkGetter<IMaterialObjectGetter>, IMaterialObjectGetter> MaterialMapping(ISkyrimModGetter bds)
        {
            Dictionary<IFormLinkGetter<IMaterialObjectGetter>, IMaterialObjectGetter> mappings = new();

            // SnowMaterialObjectNoise1P
            if (bds.MaterialObjects.TryGetValue(BDSModKey.MakeFormKey(0x39f5), out var material))
            {
                mappings.Add(SSEForms.Skyrim.MaterialObject.SnowMaterialObjectNoise1P, material);
            }
            // SnowMaterialObject1P
            if (bds.MaterialObjects.TryGetValue<IMaterialObjectGetter>(BDSModKey.MakeFormKey(0x1305), out material))
            {
                mappings.Add(SSEForms.Skyrim.MaterialObject.SnowMaterialObject1P, material);
            }
            // SnowMaterialMountain1P
            if (bds.MaterialObjects.TryGetValue<IMaterialObjectGetter>(BDSModKey.MakeFormKey(0x1868), out material))
            {
                mappings.Add(SSEForms.Skyrim.MaterialObject.SnowMaterialMountain1P, material);
            }
            // SnowMaterialObjectCustom03_1P
            if (bds.MaterialObjects.TryGetValue<IMaterialObjectGetter>(BDSModKey.MakeFormKey(0x39f4), out material))
            {
                mappings.Add(SSEForms.Skyrim.MaterialObject.SnowMaterialObjectCustom03_1P, material);
            }
            // SnowMaterialMountainTrimLight1P
            // xEdit Script does not check the Form ID and so would not map this
            if (bds.MaterialObjects.TryGetValue<IMaterialObjectGetter>(BDSModKey.MakeFormKey(0x2372), out material))
            {
                mappings.Add(SSEForms.Skyrim.MaterialObject.SnowMaterialMountainTrimLight1P, material);
            }
            // SnowMaterialWinterhold
            if (bds.MaterialObjects.TryGetValue<IMaterialObjectGetter>(BDSModKey.MakeFormKey(0x2373), out material))
            {
                mappings.Add(SSEForms.Skyrim.MaterialObject.SnowMaterialWinterhold, material);
            }
            // SnowMaterialMarkarthCliffs
            if (bds.MaterialObjects.TryGetValue<IMaterialObjectGetter>(BDSModKey.MakeFormKey(0x1304), out material))
            {
                mappings.Add(SSEForms.Skyrim.MaterialObject.SnowMaterialMarkarthCliffs, material);
            }
            // SnowMaterialObjectLight1P
            if (bds.MaterialObjects.TryGetValue<IMaterialObjectGetter>(BDSModKey.MakeFormKey(0x44b8), out material))
            {
                mappings.Add(SSEForms.Skyrim.MaterialObject.SnowMaterialObjectLight1P, material);
            }
            // SnowMaterialFarm
            if (bds.MaterialObjects.TryGetValue<IMaterialObjectGetter>(BDSModKey.MakeFormKey(0x12ef), out material))
            {
                mappings.Add(SSEForms.Skyrim.MaterialObject.SnowMaterialFarm, material);
            }
            // SnowMaterialStockade1P
            if (bds.MaterialObjects.TryGetValue<IMaterialObjectGetter>(BDSModKey.MakeFormKey(0x12f0), out material))
            {
                mappings.Add(SSEForms.Skyrim.MaterialObject.SnowMaterialStockade1P, material);
            }
            // SnowMaterialMountainTrim1P
            if (bds.MaterialObjects.TryGetValue<IMaterialObjectGetter>(BDSModKey.MakeFormKey(0x12f1), out material))
            {
                mappings.Add(SSEForms.Skyrim.MaterialObject.SnowMaterialMountainTrim1P, material);
            }
            // SnowMaterialRoadLight1P
            if (bds.MaterialObjects.TryGetValue<IMaterialObjectGetter>(BDSModKey.MakeFormKey(0x2374), out material))
            {
                mappings.Add(SSEForms.Skyrim.MaterialObject.SnowMaterialRoadLight1P, material);
            }
            // SnowMaterialObjectCustom01_1P
            if (bds.MaterialObjects.TryGetValue<IMaterialObjectGetter>(BDSModKey.MakeFormKey(0x39f2), out material))
            {
                mappings.Add(SSEForms.Skyrim.MaterialObject.SnowMaterialObjectCustom01_1P, material);
            }
            // SnowMaterialMountainLight1P
            if (bds.MaterialObjects.TryGetValue<IMaterialObjectGetter>(BDSModKey.MakeFormKey(0x2361), out material))
            {
                mappings.Add(SSEForms.Skyrim.MaterialObject.SnowMaterialMountainLight1P, material);
            }
            // SnowMaterialObjectCustom02_1P
            if (bds.MaterialObjects.TryGetValue<IMaterialObjectGetter>(BDSModKey.MakeFormKey(0x39f3), out material))
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
            var materialMapping = MaterialMapping(bdsMod.Mod);
            var skipMods = Implicits.Get(state.PatchMod.GameRelease).Listings;
            skipMods.Add(USSEPModKey);

            Console.WriteLine("{0} STAT", state.LoadOrder.PriorityOrder.WinningOverrides<IStaticGetter>().Count<IStaticGetter>());
            // skip STATs where winning override is from excluded mods
            foreach (var target in state.LoadOrder.PriorityOrder.WinningOverrides<IStaticGetter>().
                Where(stat => !skipMods.Contains(stat.FormKey.ModKey)))
            {

                if (!materialMapping.TryGetValue(target.Material, out IMaterialObjectGetter? mapped) || mapped == null)
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
