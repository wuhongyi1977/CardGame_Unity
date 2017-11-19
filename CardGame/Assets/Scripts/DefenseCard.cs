using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class DefenseCard : Card
{
    protected int def;

    //csv関連(using System & using System.IOも追加)
    private string csvFilePath = "def";
    private string[] csvDatas = new string[] { };

    public DefenseCard()
    {
        this.def = 0;
    }

    public DefenseCard(string id, string name, string attribute, string explain, int def, string illust)
    {
        this.card_id = id;
        this.card_name = name;
        this.card_attribute = attribute;
        this.card_explain = explain;
        this.def = def;
        this.card_color = Color.blue;
        this.card_illust = illust;
    }

    //IDに対応した数字からcsvを読み込むコンストラクタ
    public DefenseCard(int num)
    {
        CsvReader csvReader = new CsvReader();
        csvDatas = csvReader.Readcsv(csvFilePath, num);

        this.card_id = csvDatas[0];
        this.card_name = csvDatas[1];
        this.card_attribute = csvDatas[2];
        this.card_explain = csvDatas[3];
        this.def = Convert.ToInt32(csvDatas[4]);
        this.card_color = Color.blue;
        // 現時点でDEF.csvに無いのでとりあえず5番目に
        this.card_illust = csvDatas[5];
    }

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

}
