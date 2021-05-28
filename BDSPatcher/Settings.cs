using System;
using System.Collections.Generic;
using System.Linq;
using Mutagen.Bethesda;
using Mutagen.Bethesda.Skyrim;
using Mutagen.Bethesda.Synthesis;
using Mutagen.Bethesda.Synthesis.Settings;

namespace BDSPatcher
{
    public class Settings
    {
        [SynthesisSettingName("For Better Dynamic Snow version (display only):")]
        public string bdsVersion { get; } = "2.11.0";

        private List<string[]> _nifBlackList = new List<string[]>();
        private static List<string> DefaultBlackList()
        {
            var defaults = new List<string>();
            // For Capital Windhelm SSE - skip NIF that causes CTD after patching with Synthesis or scripts
            defaults.Add("SurWindhelmCustomMeshes,Experimental,whcenterNoColB2");
            // For Capital Windhelm SSE - skip massive NIF that goes invisible when you get close after patching with Synthesis
            defaults.Add("SurWindhelmCustomMeshes,00NewPalaceV3OptCol");
            return defaults;
        }
        private static readonly List<string[]> _defaultNifBlackList = NifFilters.ParseNifFilters(DefaultBlackList());
        [SynthesisSettingName("BlackList Patterns")]
        [SynthesisTooltip("Each entry is a comma-separated list of strings. Every string must match for a mesh to be excluded.")]
        [SynthesisDescription("List of patterns for excluded mesh names.")]
        public List<string> NifBlackList
        {
            get { return NifFilters.BuildNifFilters(_nifBlackList); }
            set { _nifBlackList = NifFilters.ParseNifFilters(value); }
        }
        private List<string[]>? _fullBlackList;
        private List<string[]> fullBlackList
        {
            get
            {
                if (_fullBlackList is null)
                    _fullBlackList = new List<string[]>(_nifBlackList.Concat(_defaultNifBlackList));
                return _fullBlackList;
            }
        }

        private List<string> _modsTrusted= new List<string>();
        private static List<string> DefaultModsTrusted()
        {
            var defaults = new List<string>();
            defaults.Add("WiZkiD Grass and Landscapes.");
            return defaults;
        }
        private static readonly List<string> _defaultModsTrusted = DefaultModsTrusted();
        [SynthesisSettingName("Mods with better snow than BDS v2")]
        [SynthesisTooltip("Each entry is a string matching the name of a mod that has proper snow, and should be forwarded to the patch. Prefer 'MyModName.' to avoid having multiple entries for esl/esp/esm variants.")]
        [SynthesisDescription("List of names of mods that have better snow than BDS v2.")]
        public List<string> ModsTrusted
        {
            get { return _modsTrusted; }
            set { _modsTrusted = value; }
        }
        private List<string>? _fullModsTrusted;
        private List<string> fullModsTrusted
        {
            get
            {
                if (_fullModsTrusted is null)
                {
                    // filter out mods that are not matched in the Load Order
                    _fullModsTrusted = new List<string>(_modsTrusted.Concat(_defaultModsTrusted));
                }
                return _fullModsTrusted;
            }
        }

        public IStaticGetter CheckTrusted(IPatcherState<ISkyrimMod, ISkyrimModGetter> state, IStaticGetter target, out bool updated)
        {
            updated = false;
            if (fullModsTrusted.Count > 0)
            {
                // check mods with better snow for this STAT record, use that target in the patch if present
                IFormLinkGetter<IStaticGetter> statLink = target.AsLinkGetter();
                var previousOverrides = statLink.ResolveAllContexts<ISkyrimMod, ISkyrimModGetter, IStatic, IStaticGetter>(state.LinkCache);
                foreach (string modFilter in fullModsTrusted)
                {
                    var candidates = previousOverrides.Where(link => link.ModKey.FileName.Contains(modFilter, StringComparison.OrdinalIgnoreCase));
                    if (candidates.Count() > 1)
                    {
                        Console.WriteLine("Trusted mod filter {0} matches {1} previous overrides for {2}/{3:X8}, should be 0 or 1",
                            modFilter, candidates.Count(), target.FormKey.ModKey.FileName, target.FormKey.ID);
                    }
                    else if (candidates.Count() == 1)
                    {
                        updated = true;
                        return candidates.First().Record;
                    }
                }
            }
            return target;
        }

        public bool IsNifValid(string nifPath)
        {
            // check blacklist, exclude NIF if all substrings in an entry match
            foreach (string[] filterElements in fullBlackList)
            {
                if (filterElements
                    .Where(x => !string.IsNullOrEmpty(x))
                    .All(v => nifPath.Contains(v, StringComparison.OrdinalIgnoreCase)))
                    return false;
            }
            // if not blacklisted, good to go
            return true;
        }
    }
}
