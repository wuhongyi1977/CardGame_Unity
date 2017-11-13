using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class DefenseCard : Card
{

    protected int def1;
    protected int def2;
    protected int def3;

    //csv関連(using System & using System.IOも追加)
    private string csvFilePath = "def";
    private string[] csvDatas = new string[] { };

    public DefenseCard()
    {
        this.def1 = 0;
        this.def2 = 0;
        this.def3 = 0;
    }

    public DefenseCard(string name, string id, string explain, string illust, int def1, int def2, int def3)
    {
        this.card_name = name;
        this.card_id = id;
        this.card_explain = explain;
        this.card_color = Color.blue;
        this.card_illust = illust;
        this.def1 = def1;
        this.def2 = def2;
        this.def3 = def3;
    }

    //IDに対応した数字からcsvを読み込むコンストラクタ
    public DefenseCard(int num)
    {
        CsvReader csvReader = new CsvReader();
        csvDatas = csvReader.Readcsv(csvFilePath, num);

        this.card_id = csvDatas[0];
        this.card_name = csvDatas[1];
        this.card_explain = csvDatas[2];
        this.card_color = Color.blue;
        this.card_illust = csvDatas[3];
        this.def1 = Convert.ToInt32(csvDatas[4]);
        this.def2 = Convert.ToInt32(csvDatas[5]);
        this.def3 = Convert.ToInt32(csvDatas[6]);

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
