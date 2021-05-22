using System;
using System.Collections.Generic;
using System.Linq;
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
