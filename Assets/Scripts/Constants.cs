using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets.Scripts
{

    public static class RangeConstants
    {
        /// <summary>
        /// T1 Constant Ranges
        /// </summary>

        //grumble (muttering) index range constants
        public const int grumble1_Boss = 0;
        public const int grumble2_Company = 4;
        public const int grumble3_Cops = 8;
        public const int grumble4_BadGuy = 13;
        public const int grumble5_Stupid = 17;
        public const int grumble6_Unbelievable = 22;
        public const int grumble7_WhatDoing = 27;

        //Hey You index ranges
        public const int heyYou_A1 = 0;
        public const int heyYou_A2 = 1;
        public const int heyYou_A3 = 3;

        //Insult index range
        public const int insult_A1 = 0;
        public const int insult_A2 = 2;
        public const int insult_A3 = 4;

        //Name index range
        public const int name_A1 = 0;
        public const int name_A2 = 1;
        public const int name_A3 = 2;

        //Purpose index range
        public const int purpose_A1 = 0;
        public const int purpose_A2 = 1;
        public const int purpose_A3 = 3;
        public static readonly int[] longClipPurpose = { 2 };

        //Resist index range
        public const int resist_A1 = 0;
        public const int resist_A2 = 3;
        public const int resist_A3 = 6;
        public static readonly int[] longClipResist = { 3, 4, 7, 8 };

        //StepOut index range
        public const int stepOut_A1 = 0;
        public const int stepOut_A2 = 2;
        public const int stepOut_A3 = 4;

        //TalkReason index range
        public const int talkReason_A1 = 0;
        public const int talkReason_A2 = 1;
        public const int talkReason_A3 = 3;

        //Approach index range
        public const int approach_A1 = 0;
        public const int approach_A2 = 2;
        public const int approach_A3 = 4;

        //Leave count
        public const int leave_count = 3;
        public static readonly int[] longClipLeave = { 0, 1 };

        //LosPass count
        public const int losPass_count = 6;
        public static readonly int[] longClipLosPass = { 2, 3, 4, 5 };

        //Talk count
        public const int talk_count = 3;

        //Remove count
        public const int remove_count = 3;

        //Remove Persist count
        public const int removePersist_count = 3;


        /// <summary>
        /// T2 Constant Ranges
        /// </summary>

        //AssureReprimand count
        public const int assureReprimand_count = 5;

        //CalmDown count
        public const int calmDown_count = 1;

        //Confide count
        public const int confide_count = 3;

        //Dismiss count
        public const int dismiss_count = 5;

        //Focus count
        public const int focus_count = 9;

        //Purpose count
        public const int purpose_count = 2;

        //Resist count
        public const int resist_count = 3;

        //Title count
        public const int title_count = 1;

        //Outside drop stuff
        public const int dropStuff_index = 0;

    }

    public enum T1_TAG_INDEX { EMPTY, CALMDOWN, INSULT, RESIST, QUESTION, NAME, HEYYOU, PURPOSE, LOSPASS, STEPOUT, TALKREASON,
        TALK, APPROACH, REMOVEPERSIST, REMOVE, NONE };

    public enum T2_TAG_INDEX { EMPTY, CALMDOWN, RESIST, TITLE, PURPOSE, FOCUS, DISMISS, ASSURE, CONFIDE, NONE };

    public static class TagMatrix
    {
        public static readonly Dictionary<string, T1_TAG_INDEX> T1_TagConversion= new Dictionary<string, T1_TAG_INDEX>
            {
                { "CalmDown", T1_TAG_INDEX.CALMDOWN },
                { "Insult", T1_TAG_INDEX.INSULT },
                { "Resist", T1_TAG_INDEX.RESIST },
                { "Question", T1_TAG_INDEX.QUESTION },
                { "Name", T1_TAG_INDEX.NAME },
                { "HeyYou", T1_TAG_INDEX.HEYYOU },
                { "Purpose", T1_TAG_INDEX.PURPOSE },
                { "LosPass", T1_TAG_INDEX.LOSPASS },
                { "StepOut", T1_TAG_INDEX.STEPOUT },
                { "TalkReason", T1_TAG_INDEX.TALKREASON },
                { "Talk", T1_TAG_INDEX.TALK },
                { "Approach", T1_TAG_INDEX.APPROACH },
                { "RemovePersist", T1_TAG_INDEX.REMOVEPERSIST },
                { "Remove", T1_TAG_INDEX.REMOVE },
                { "NONE", T1_TAG_INDEX.NONE }
            };

        public static readonly string[,] T1_Tags = 
        {    { "EMPTY", "CalmDown", "Insult", "Resist", "Question", "Name", "HeyYou", "Purpose", "LosPass", "StepOut", "TalkReason", "Talk", "Approach", "RemovePersist", "Remove", "NONE" },
          { "CalmDown", "CalmDown", "Insult", "Resist", "Question", "Name", "HeyYou", "Purpose", "LosPass", "StepOut", "TalkReason", "Talk", "Approach", "RemovePersist", "Remove", "CalmDown" },
          { "Insult",   "CalmDown", "Insult", "Resist", "Question", "Name", "HeyYou", "Purpose", "LosPass", "StepOut", "TalkReason", "Talk", "Approach", "RemovePersist", "Remove", "Insult" },
          { "Resist",   "CalmDown", "Resist", "Resist", "Question", "Name", "HeyYou", "Purpose", "LosPass", "StepOut", "TalkReason", "Talk", "Approach", "RemovePersist", "Remove", "Resist" },
          { "Question", "CalmDown", "Insult", "Resist", "Question", "Name", "HeyYou", "Purpose", "LosPass", "StepOut", "TalkReason", "Talk", "Approach", "RemovePersist", "Remove", "Question" },
          { "Name",     "CalmDown", "Insult", "Resist", "Question", "Name", "HeyYou", "Purpose", "LosPass", "StepOut", "TalkReason", "Talk", "Approach", "RemovePersist", "Remove", "Name" },
          { "HeyYou",   "CalmDown", "Insult", "Resist", "Question", "Name", "HeyYou", "Purpose", "LosPass", "StepOut", "TalkReason", "Talk", "Approach", "RemovePersist", "Remove", "HeyYou" },
          { "Purpose",  "CalmDown", "Insult", "Resist", "Question", "Name", "HeyYou", "Purpose", "LosPass", "StepOut", "TalkReason", "Talk", "Approach", "RemovePersist", "Remove", "Purpose" },
          { "LosPass",  "CalmDown", "Insult", "Resist", "Question", "Name", "HeyYou", "Purpose", "LosPass", "StepOut", "TalkReason", "Talk", "Approach", "RemovePersist", "Remove", "LosPass" },
          { "StepOut",  "CalmDown", "Insult", "Resist", "Question", "Name", "HeyYou", "Purpose", "LosPass", "StepOut", "TalkReason", "StepOut", "Approach", "RemovePersist", "Remove", "StepOut" },
          { "TalkReason", "CalmDown", "Insult", "Resist", "Question", "Name", "HeyYou", "Purpose", "LosPass", "StepOut", "TalkReason", "TalkReason", "Approach", "RemovePersist", "Remove", "TalkReason" },
          { "Talk",      "CalmDown", "Insult", "Resist", "Question", "Name", "HeyYou", "Purpose", "LosPass", "StepOut", "TalkReason", "Talk", "Approach", "RemovePersist", "Remove", "Talk" },
          { "Approach", "CalmDown", "Insult", "Resist", "Question", "Name", "HeyYou", "Purpose", "LosPass", "StepOut", "TalkReason", "Talk", "Approach", "RemovePersist", "Remove", "Approach" },
          { "RemovePersist", "CalmDown", "Insult", "Resist", "Question", "Name", "HeyYou", "Purpose", "LosPass", "StepOut", "TalkReason", "Talk", "Approach", "RemovePersist", "Remove", "RemovePersist" },
          { "Remove", "CalmDown", "Insult", "Resist", "Question", "Name", "HeyYou", "Purpose", "LosPass", "StepOut", "TalkReason", "Talk", "Approach", "RemovePersist", "Remove", "Remove" },
          { "NONE", "CalmDown", "Insult", "Resist", "Question", "Name", "HeyYou", "Purpose", "LosPass", "StepOut", "TalkReason", "Talk", "Approach", "RemovePersist", "Remove", "NONE" },
        };

        public static readonly Dictionary<string, T2_TAG_INDEX> T2_TagConversion = new Dictionary<string, T2_TAG_INDEX>
        {
            { "CalmDown", T2_TAG_INDEX.CALMDOWN },
            { "Resist", T2_TAG_INDEX.RESIST },
            { "Title", T2_TAG_INDEX.TITLE},
            { "Purpose", T2_TAG_INDEX.PURPOSE },
            { "Focus", T2_TAG_INDEX.FOCUS },
            { "Dismiss", T2_TAG_INDEX.DISMISS},
            { "AssureReprimand", T2_TAG_INDEX.ASSURE },
            { "Confide", T2_TAG_INDEX.CONFIDE },
            { "NONE", T2_TAG_INDEX.NONE }
        };

        public static readonly string[,] T2_Tags =
        {
               { "EMPTY", "CalmDown", "Resist", "Title", "Purpose", "Focus", "Dismiss", "AssureReprimand", "Confide", "NONE"},
            { "CalmDown", "CalmDown", "Resist", "CalmDown", "Purpose", "Focus", "Dismiss", "AssureReprimand", "Confide", "NONE"},
            { "Resist",   "Resist",   "Resist", "Resist", "Purpose", "Resist", "Dismiss", "AssureReprimand", "Confide", "NONE"},
            { "Title",    "CalmDown", "Resist", "Title", "Purpose", "Focus", "Dismiss", "AssureReprimand", "Confide", "NONE"},
            { "Purpose",  "CalmDown", "Resist", "Title", "Purpose", "Focus", "Dismiss", "AssureReprimand", "Confide", "NONE"},
            { "Focus",    "CalmDown", "Resist", "Title", "Purpose", "Focus", "Focus", "AssureReprimand", "Confide", "NONE"},
            { "Dismiss",  "CalmDown", "Resist", "Title", "Purpose", "Focus", "Focus", "AssureReprimand", "Confide", "NONE"},
            { "AssureReprimand",  "CalmDown", "Resist", "Title", "Purpose", "AssureReprimand", "AssureReprimand", "AssureReprimand", "Confide", "NONE"},
            { "Confide",  "CalmDown", "Resist", "Title", "Purpose", "Focus", "Dismiss", "AssureReprimand", "Confide", "NONE"},
            { "NONE",     "CalmDown", "Resist", "Title", "Purpose", "Focus", "Dismiss", "AssureReprimand", "Confide", "NONE"}

        };


    }
}
