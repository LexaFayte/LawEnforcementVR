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
}
