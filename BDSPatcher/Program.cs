using System;
using System.Collections.Generic;
using System.Linq;
using Mutagen.Bethesda;
using SSEForms = Mutagen.Bethesda.FormKeys.SkyrimSE;
using Mutagen.Bethesda.Synthesis;
using Mutagen.Bethesda.Skyrim;
using System.Threading.Tasks;
using Noggog;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Plugins.Order;

namespace BDSPatcher
{
    public class Program
    {
        static Lazy<Settings> _settings = null!;
        static public Settings settings => _settings.Value;
        public static async Task<int> Main(string[] args)
        {
            return await SynthesisPipeline.Instance
                .SetTypicalOpen(GameRelease.SkyrimSE, "BDSPatcher.esp")
                .SetAutogeneratedSettings("Settings", "settings.json", out _settings)
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
        private static IPatcherState<ISkyrimMod, ISkyrimModGetter>? _state;
        internal static IPatcherState<ISkyrimMod, ISkyrimModGetter> State
        {
            get { return _state!; }
        }
        public static void RunPatch(IPatcherState<ISkyrimMod, ISkyrimModGetter> state)
        {
            _state = state;
            if (!state.LoadOrder.TryGetValue(BDSModKey, out IModListing<ISkyrimModGetter>? bdsMod) || bdsMod == null || bdsMod.Mod == null)
            {
                throw new ArgumentException("Unable to get Better Dynamic Snow.esp plugin");
            }
            var materialMapping = MaterialMapping(bdsMod.Mod);
            var skipMods = Implicits.Get(state.PatchMod.GameRelease).Listings.ToHashSet();
            skipMods.Add(USSEPModKey);

            var getters = state.LoadOrder.PriorityOrder.WinningOverrides<IStaticGetter>();
            Console.WriteLine("{0} STAT", getters.Count<IStaticGetter>());
            // skip STATs where winning override is from excluded mods,or NIF is blacklisted
            foreach (var target in getters)
            {
                if (target.Model != null && target.Model.File != null)
                {
                    if (!settings.IsNifValid(target.Model.File))
                    {
                        Console.WriteLine("Skip blacklisted NIF {0} for STAT {1}:{2}/{3:X8}",
                            target.Model.File, target.FormKey.ModKey.FileName,
                            target.EditorID, target.FormKey.ID);
                        continue;
                    }
                }
                // we need to introspect the provenance of the record
                var contexts = state.LinkCache.ResolveAllContexts<IStatic, IStaticGetter>(target.FormKey).ToList();
                var currentWinner = contexts[0];
                // Do not patch winning override from game files or USSEP
                if (skipMods.Contains(currentWinner.ModKey))
                {
                    Console.WriteLine("Skip STAT {0}/{1:X8} with winning override in '{2}'",
                        target.EditorID, target.FormKey.ID, target.FormKey.ModKey.FileName);
                    continue;
                }

                // Check whether we want to force override with a trusted mod's snow MATO
                var trueContext = settings.CheckTrusted(contexts, target, out var trusted, out var filename);
                var trueWinner = trueContext.Record;
                if (trusted)
                {
                    // Use trusted mod MATO. If it's the winning override then no-op, to avoid ITPO.
                    if (trueContext != currentWinner)
                    {
                        Console.WriteLine("Force-promote STAT {0}:{1}/{2:X8} from trusted mod '{3}'",
                            trueContext.ModKey.FileName, trueWinner.EditorID, trueWinner.FormKey.ID, filename);
                        state.PatchMod.Statics.GetOrAddAsOverride(trueWinner);
                    }
                    else
                    {
                        Console.WriteLine("STAT {0}:{1}/{2:X8} from trusted mod '{3}' is already the winning override",
                            trueWinner.FormKey.ModKey.FileName, trueWinner.EditorID, trueWinner.FormKey.ID, filename);
                    }
                    continue;
                }
                else
                {
                    // MATO mapping may be required
                    if (!materialMapping.TryGetValue(trueWinner.Material, out IMaterialObjectGetter? mapped) || mapped == null)
                    {
                        continue;
                    }
                    var newStatic = state.PatchMod.Statics.GetOrAddAsOverride(trueWinner);
                    var matName = trueContext.Record.Material;
                    Console.WriteLine("MATO {0:X8} mapped to BDS {1:X8} in STAT {2}:{3}/{4:X8}",
                        matName.FormKey.ID, mapped.FormKey.ID, trueWinner.FormKey.ModKey.FileName, trueWinner.EditorID, trueWinner.FormKey.ID);
                    newStatic.Material = new FormLink<IMaterialObjectGetter>(mapped.FormKey);
                }
            }
        }
    }
}
