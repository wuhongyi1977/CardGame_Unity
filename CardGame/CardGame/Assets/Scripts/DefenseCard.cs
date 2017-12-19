using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class DefenseCard : Card
{
    protected int def_c;
    protected int def_i;
    protected int def_a;

    //csv関連(using System & using System.IOも追加)
    private string csvFilePath = "def";
    private string[] csvDatas = new string[] { };

    public DefenseCard()
    {
        this.def_c = 0;
        this.def_i = 0;
        this.def_a = 0;
    }

    public DefenseCard(string id, string name, string attribute, string describe, int c, int i, int a, string illust)
    {
        this.card_id = id;
        this.card_name = name;
        this.card_attribute = attribute;
        this.card_describe = describe;
        if (this.card_attribute.Equals("Confidentiality"))
        { // 機密性
            this.def_c = c;
            this.def_i = 0;
            this.def_a = 0;
        } else if (this.card_attribute.Equals("Integrity"))
        { // 完全性
            this.def_c = 0;
            this.def_i = i;
            this.def_a = 0;
        } else if (this.card_attribute.Equals("Availability"))
        { // 可用性
            this.def_c = 0;
            this.def_i = 0;
            this.def_a = a;
        } else if (this.card_attribute.Equals("Overall"))
        { // 全属性
            this.def_c = c;
            this.def_i = i;
            this.def_a = a;
        }
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
        this.card_describe = csvDatas[3];
        if (this.card_attribute.Equals("Confidentiality"))
        { // 機密性
            this.def_c = Convert.ToInt32(csvDatas[4]);
            this.def_i = 0;
            this.def_a = 0;
        }
        else if (this.card_attribute.Equals("Integrity"))
        { // 完全性
            this.def_c = 0;
            this.def_i = Convert.ToInt32(csvDatas[4]);
            this.def_a = 0;
        }
        else if (this.card_attribute.Equals("Availability"))
        { // 可用性
            this.def_c = 0;
            this.def_i = 0;
            this.def_a = Convert.ToInt32(csvDatas[4]);
        }
        else if (this.card_attribute.Equals("Overall"))
        { // 全属性
            this.def_c = Convert.ToInt32(csvDatas[4]);
            this.def_i = Convert.ToInt32(csvDatas[4]);
            this.def_a = Convert.ToInt32(csvDatas[4]);
        }
        this.card_color = Color.blue;
        // 現時点でdef.csvに無いのでとりあえず5番目に
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
