using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class SemanticsParser {

    /// <summary>
    /// parses the semantics string, outputting a list of the separate identifiers the user said
    /// </summary>
    /// <param name="toParse">string to parse</param>
    /// <param name="semantics">list to store the semantic identifiers</param>
    public static void parse(string toParse, ref List<string> semantics)
    {
        if(toParse.Contains("[object Object]"))
        {
            toParse = removeGarbage(toParse);
        }

        string[] splits = toParse.Split('_');
        semantics = splits.ToList();
        semantics.Remove("");
        semantics = semantics.Distinct().ToList();
    }

    /// <summary>
    /// removes the "[object Object]" string from the semantics
    /// </summary>
    /// <param name="s">string to parse</param>
    /// <returns>new string with garbage tokens removed</returns>
    private static string removeGarbage(string s)
    {
        string final = s.Replace("[object Object]", "");
        return final;
    }



}
