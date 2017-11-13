﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class AttackCard : Card {

    protected int atk;

    //csv関連(using System & using System.IOも追加)
    private string csvFilePath="atk";
    private TextAsset csvFile;
    private string[] csvDatas = new string[] {};

    public AttackCard(){
        this.atk = 0;
    }

    public AttackCard(string name, string id, string explain, string illust, int atk){
        this.card_name = name;
        this.card_id = id;
        this.card_explain = explain;
        this.card_color = Color.red;
        this.card_illust = illust;
        this.atk = atk;
    }

    //IDに対応した数字からcsvを読み込むコンストラクタ
    public AttackCard(int num){
        CsvReader csvReader = new CsvReader();
        csvDatas = csvReader.Readcsv(csvFilePath,num);

        this.card_id = csvDatas[0];
        this.card_name = csvDatas[1];
        this.card_explain = csvDatas[2];
        this.card_color = Color.red;
        this.card_illust = csvDatas[3];
        this.atk = Convert.ToInt32(csvDatas[4]);

    }

	// Use this for initialization
	void Start () {
        
    }
	
	// Update is called once per frame
	void Update () {
		
	}

}
