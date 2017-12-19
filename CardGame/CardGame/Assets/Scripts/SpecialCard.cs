using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System;

public class SpecialCard : Card {

    protected int spe_sp;
    private string csvFilePath = "spe";
    private string[] csvDatas = new string[] { };

    public SpecialCard()
    {
        this.spe_sp = 0;
    }

    public SpecialCard(string id, string name,string describe, string illust)
    {
        this.card_id = id;
        this.card_name = name;
        this.card_describe = describe;
        this.card_color = Color.yellow;
        this.card_illust = illust;
    }

    public SpecialCard(int num)
    {
        CsvReader csvReader = new CsvReader();
        csvDatas = csvReader.Readcsv(csvFilePath, num);

        this.card_id = csvDatas[0];
        this.card_name = csvDatas[1];
        this.card_describe = csvDatas[2];
        this.card_color = Color.yellow;
        // 現時点でdef.csvに無いのでとりあえず5番目に
        this.card_illust = csvDatas[5];

    }
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
