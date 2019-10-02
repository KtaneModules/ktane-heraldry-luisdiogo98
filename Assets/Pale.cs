using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using KModkit;
using rnd = UnityEngine.Random;

class Pale : Crest {

    public Pale(List<int> validFields, List<int> validCharges, bool valid, bool fake = false, bool unicorn = false, bool royal = false)
    {
        this.validFields = validFields;
        this.validCharges = validCharges;
        this.valid = valid;
        this.fake = fake;
        this.unicorn = unicorn;
        this.royal = royal;

        do Generate(); while(CheckForbidden());
    }

    public override string Description()
    {
        return "Arms of " + familyName + ": Three " + fieldNames[chargeColors[0]] + " " + chargeNames[charges[0]] + " over a " + fieldNames[fields[0]] + " Pale and a " + fieldNames[fields[1]] + " field" + (fake ? " [BREAKS RULE OF TINCTURE]" : "") + (unicorn ? " [ORDER OF THE UNICORN]" : "") + (royal ? " [ROYAL HOUSE]" : "");
    }

    public override void Paint(GameObject shield, Material[] materials)
    {
        char[] name = familyName.ToArray();
        if(name.Length > 15)
            name[familyName.LastIndexOf(' ')] = '\n';

        shield.transform.Find("name").GetComponentInChildren<TextMesh>().text = name.Join("");

        shield.transform.GetComponentInChildren<Renderer>().material = materials.Where(x => x.name == fieldNames[fields[1]].ToLower()).ToArray()[0];

        shield.transform.Find("divisions").Find("pale").gameObject.SetActive(true);
        shield.transform.Find("divisions").Find("pale").GetComponentInChildren<Renderer>().material = materials.Where(x => x.name == fieldNames[fields[0]].ToLower()).ToArray()[0];
    
        shield.transform.Find("charges").Find("charge1_pale").gameObject.SetActive(true);
        shield.transform.Find("charges").Find("charge1_pale").GetComponentInChildren<Renderer>().material = materials.Where(x => x.name == chargeNamesMat[charges[0]].ToLower() + "_" + fieldNames[chargeColors[0]].ToLower()).ToArray()[0];

        shield.transform.Find("charges").Find("charge2_pale").gameObject.SetActive(true);
        shield.transform.Find("charges").Find("charge2_pale").GetComponentInChildren<Renderer>().material = materials.Where(x => x.name == chargeNamesMat[charges[0]].ToLower() + "_" + fieldNames[chargeColors[0]].ToLower()).ToArray()[0];
        
        shield.transform.Find("charges").Find("charge3_pale").gameObject.SetActive(true);
        shield.transform.Find("charges").Find("charge3_pale").GetComponentInChildren<Renderer>().material = materials.Where(x => x.name == chargeNamesMat[charges[0]].ToLower() + "_" + fieldNames[chargeColors[0]].ToLower()).ToArray()[0];
    }
    
    public override void Generate()
    {
        GenericGenerate();
        if(valid || fake)
            fields.Add(GetRandomValidTincture(validFields.ToArray(), new int[] {fields[0]}));
        else
            fields.Add(GetRandomTincture(new int[] {fields[0]}));
    }

    public override int GetScore(KMBombInfo bomb)
    {
        int divisionScore = 2 * 3 - 0 + 3;
        
        bool foundGAV = false;
        bool foundPSC = false;
        bool foundStain = false;

        for(int i = 0; i < fields.Count(); i++)
            if(fields[i] == GULES || fields[i] == AZURE || fields[i] == VERT)
                foundGAV = true;
            else if(fields[i] == PURPURE || fields[i] == SABLE || fields[i] == CELESTE)
                foundPSC = true;
            else if(fields[i] == SANGUINE || fields[i] == MURREY || fields[i] == TENNE)
                foundStain = true;

        for(int i = 0; i < chargeColors.Count(); i++)
            if(chargeColors[i] == GULES || chargeColors[i] == AZURE || chargeColors[i] == VERT)
                foundGAV = true;
            else if(chargeColors[i] == PURPURE || chargeColors[i] == SABLE || chargeColors[i] == CELESTE)
                foundPSC = true;
            else if(chargeColors[i] == SANGUINE || chargeColors[i] == MURREY || chargeColors[i] == TENNE)
                foundStain = true;

        int tinctureScore = (foundGAV ? 2 : 0) + (foundPSC ? -1 : 0) + (foundStain ? 5 : 0);

        int chargeScore = 3 * ((charges[0] <= BOTTONY ? 1 : 0) + (charges[0] > BOTTONY ? -1 : 0));

        int familyScore = familyName.Length - familyName.Where(x => x == ' ' || x == '\'').Count() - (familyName.Where(x => x == ' ').Count() + 1) + 4 * bomb.GetSerialNumberLetters().Where(x => familyName.ToUpperInvariant().ToArray().Contains(x)).Count(); 
    
        return divisionScore + tinctureScore + chargeScore + familyScore;
    }
}