﻿// FactionUtilities.cs modified by Iron Wolf for Pawnmorph on 10/06/2019 12:46 PM
// last updated 10/06/2019  12:46 PM

using System;
using JetBrains.Annotations;
using RimWorld;
using Verse;

namespace Pawnmorph
{
    /// <summary> Static container for faction related utilities. </summary>
    public static class FactionUtilities
    {
        private const int TRANSFORMED_RELATIONSHIP_OFFSET = -10;//TODO put this in a def or something 
        private const string GOODWILL_LABEL = "GoodwillChangedReason_PawnTransformed"; //the tag name to use to get translated text 
        private const string LEADER_TRANSFORMED_LABEL = "FactionLeaderTransformedLabel";
        private const string LEADER_TRANSFORMED_CONTENT = "FactionLeaderTransformedContent";
        private const string MEMBER_REVERTED = "GoodwillChangedReason_PawnReverted"; 

        private const string MEMBER_NAME = "memberName"; //these constants are used to give the params in the translation xml consistent names for all faction related 
        private const string MEMBER_LABEL = "memberLabel"; //stuff 
        private const string ANIMAL_SPECIES = "species";
        private const string OLD_LEADER = "oldLeader";
        private const string NEW_LEADER = "newLeader";
        private const string ANIMAL = "animal";
        private const string LEADER_TITLE = "leaderTitle";
        private const string FACTION_NAME = "factionName"; 


        /// <summary> Notify this faction that one of their pawns has been transformed. </summary>
        public static void Notify_MemberTransformed([NotNull] this Faction faction, [NotNull] Pawn member, [NotNull] Pawn animal, bool wasWorldPawn,  [CanBeNull] Map map)
        {
            if (faction == null) throw new ArgumentNullException(nameof(faction));
            if (member == null) throw new ArgumentNullException(nameof(member));
            if (animal == null) throw new ArgumentNullException(nameof(animal));
            if (faction.IsPlayer) return;

            if (!wasWorldPawn
             && !PawnGenerator.IsBeingGenerated(member)
             && (Current.ProgramState == ProgramState.Playing && map != null)
             && (map.IsPlayerHome && !faction.HostileTo(Faction.OfPlayer))) //check that mirrors that in Faction Notify_MemberDied 
            {
                var reason = GOODWILL_LABEL.Translate(member.LabelShort.Named(MEMBER_LABEL)
                                                      , animal.def.LabelCap.Named(ANIMAL_SPECIES)
                                                      ); // the first arg will be the former pawn, the second the animal 
                faction.TryAffectGoodwillWith(Faction.OfPlayer, TRANSFORMED_RELATIONSHIP_OFFSET, reason: reason); 
            }

            if (member == faction.leader)
            {
                Notify_LeaderTransformed(faction, animal); 
            }
        }

        public static void Notify_MemberReverted([NotNull] this Faction faction, [NotNull] Pawn member, [NotNull] Pawn animal,
                                                 bool wasWorldPawn, [CanBeNull] Map map)
        {
            if (faction == null) throw new ArgumentNullException(nameof(faction));
            if (member == null) throw new ArgumentNullException(nameof(member));
            if (animal == null) throw new ArgumentNullException(nameof(animal));
            if (faction.IsPlayer) return;

            var reason = MEMBER_REVERTED.Translate(member.LabelShort.Named(MEMBER_LABEL),
                                                   animal.def.LabelCap.Named(ANIMAL_SPECIES));
            faction.TryAffectGoodwillWith(Faction.OfPlayer, -TRANSFORMED_RELATIONSHIP_OFFSET, reason: reason); 
        }

        //TODO handle merged reactions (going to be harder because of the different possibilities)


        /// <summary> Notify this faction that it's leader has been transformed. </summary>
        public static void Notify_LeaderTransformed([NotNull] this Faction faction, Pawn animal)
        {
            var leader = faction.leader;
            faction.GenerateNewLeader();
            var newLeader = faction.leader;
            var letterLabel = LEADER_TRANSFORMED_LABEL.Translate( faction.Name.Named(FACTION_NAME), 
                                                            faction.def.leaderTitle.CapitalizeFirst().Named(LEADER_TITLE),
                                                            animal.def.LabelCap.Named(ANIMAL_SPECIES)
                                                                );
            letterLabel = letterLabel.CapitalizeFirst();
            var letterContent = LEADER_TRANSFORMED_CONTENT.Translate(leader.Name.ToStringFull.Named(MEMBER_NAME),
                                                                     animal.def.LabelCap.Named(ANIMAL_SPECIES),
                                                                     faction.Name.Named(FACTION_NAME),
                                                                     faction.def.leaderTitle.CapitalizeFirst().Named(LEADER_TITLE),
                                                                     leader.Named(OLD_LEADER),
                                                                     animal.Named(ANIMAL),
                                                                     newLeader.Named(NEW_LEADER)
                                                                    );
            letterContent = letterContent.CapitalizeFirst();
            Find.LetterStack.ReceiveLetter(letterLabel, letterContent, LetterDefOf.NegativeEvent, LookTargets.Invalid, faction);
        }
    }
}
