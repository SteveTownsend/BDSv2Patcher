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
        public static Task<int> Main(string[] args)
        {
            return SynthesisPipeline.Instance.SetTypicalOpen(GameRelease.SkyrimSE, "BDSPatcher.esp")
                .AddPatch<ISkyrimMod, ISkyrimModGetter>(RunPatch)
                .Run(args);
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
            IMaterialObjectGetter? material = IGroupMixIns.TryGetValue(bdsMaterials, new FormKey(BDSModKey, 0x39f5));
            if (material != null)
            {
                mappings.Add(SSEForms.Skyrim.MaterialObject.SnowMaterialObjectNoise1P.FormKey, material);
            }
            // SnowMaterialObject1P
            material = IGroupMixIns.TryGetValue(bdsMaterials, new FormKey(BDSModKey, 0x1305));
            if (material != null)
            {
                mappings.Add(SSEForms.Skyrim.MaterialObject.SnowMaterialObject1P.FormKey, material);
            }
            // SnowMaterialMountain1P
            material = IGroupMixIns.TryGetValue(bdsMaterials, new FormKey(BDSModKey, 0x1868));
            if (material != null)
            {
                mappings.Add(SSEForms.Skyrim.MaterialObject.SnowMaterialMountain1P.FormKey, material);
            }
            // SnowMaterialObjectCustom03_1P
            material = IGroupMixIns.TryGetValue(bdsMaterials, new FormKey(BDSModKey, 0x39f4));
            if (material != null)
            {
                mappings.Add(SSEForms.Skyrim.MaterialObject.SnowMaterialObjectCustom03_1P.FormKey, material);
            }
            // SnowMaterialMountainTrimLight1P
            // xEdit Script does not check the Form ID and so would not map this
            material = IGroupMixIns.TryGetValue(bdsMaterials, new FormKey(BDSModKey, 0x2372));
            if (material != null)
            {
                mappings.Add(SSEForms.Skyrim.MaterialObject.SnowMaterialMountainTrimLight1P.FormKey, material);
            }
            // SnowMaterialWinterhold
            material = IGroupMixIns.TryGetValue(bdsMaterials, new FormKey(BDSModKey, 0x2373));
            if (material != null)
            {
                mappings.Add(SSEForms.Skyrim.MaterialObject.SnowMaterialWinterhold.FormKey, material);
            }
            // SnowMaterialMarkarthCliffs
            material = IGroupMixIns.TryGetValue(bdsMaterials, new FormKey(BDSModKey, 0x1304));
            if (material != null)
            {
                mappings.Add(SSEForms.Skyrim.MaterialObject.SnowMaterialMarkarthCliffs.FormKey, material);
            }
            // SnowMaterialObjectLight1P
            material = IGroupMixIns.TryGetValue(bdsMaterials, new FormKey(BDSModKey, 0x44b8));
            if (material != null)
            {
                mappings.Add(SSEForms.Skyrim.MaterialObject.SnowMaterialObjectLight1P.FormKey, material);
            }
            // SnowMaterialFarm
            material = IGroupMixIns.TryGetValue(bdsMaterials, new FormKey(BDSModKey, 0x12ef));
            if (material != null)
            {
                mappings.Add(SSEForms.Skyrim.MaterialObject.SnowMaterialFarm.FormKey, material);
            }
            // SnowMaterialStockade1P
            material = IGroupMixIns.TryGetValue(bdsMaterials, new FormKey(BDSModKey, 0x12f0));
            if (material != null)
            {
                mappings.Add(SSEForms.Skyrim.MaterialObject.SnowMaterialStockade1P.FormKey, material);
            }
            // SnowMaterialMountainTrim1P
            material = IGroupMixIns.TryGetValue(bdsMaterials, new FormKey(BDSModKey, 0x12f1));
            if (material != null)
            {
                mappings.Add(SSEForms.Skyrim.MaterialObject.SnowMaterialMountainTrim1P.FormKey, material);
            }
            // SnowMaterialRoadLight1P
            material = IGroupMixIns.TryGetValue(bdsMaterials, new FormKey(BDSModKey, 0x2374));
            if (material != null)
            {
                mappings.Add(SSEForms.Skyrim.MaterialObject.SnowMaterialRoadLight1P.FormKey, material);
            }
            // SnowMaterialObjectCustom01_1P
            material = IGroupMixIns.TryGetValue(bdsMaterials, new FormKey(BDSModKey, 0x39f2));
            if (material != null)
            {
                mappings.Add(SSEForms.Skyrim.MaterialObject.SnowMaterialObjectCustom01_1P.FormKey, material);
            }
            // SnowMaterialMountainLight1P
            material = IGroupMixIns.TryGetValue(bdsMaterials, new FormKey(BDSModKey, 0x2361));
            if (material != null)
            {
                mappings.Add(SSEForms.Skyrim.MaterialObject.SnowMaterialMountainLight1P.FormKey, material);
            }
            // SnowMaterialObjectCustom02_1P
            material = IGroupMixIns.TryGetValue(bdsMaterials, new FormKey(BDSModKey, 0x39f3));
            if (material != null)
            {
                mappings.Add(SSEForms.Skyrim.MaterialObject.SnowMaterialObjectCustom02_1P.FormKey, material);
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
            var skipMods = Implicits.Get(state.PatchMod.GameRelease).Listings.ToHashSet();
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
            }
        }
    }
}
