﻿// PMTraitDefOf.cs created by Iron Wolf for Pawnmorph on 09/16/2019 12:48 PM
// last updated 09/16/2019  12:48 PM

using RimWorld;

namespace Pawnmorph
{
    /// <summary>
    /// static class containing references to commonly used Traits 
    /// </summary>
    [DefOf]
    public static class PMTraitDefOf
    {
        static PMTraitDefOf()
        {
            DefOfHelper.EnsureInitializedInCtor(typeof(TraitDefOf));
        }
        public static TraitDef MutationAffinity;
    }
}