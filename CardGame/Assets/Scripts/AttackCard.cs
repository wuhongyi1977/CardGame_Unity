using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class AttackCard : Card
{
    protected int atk_c;
    protected int atk_i;
    protected int atk_a;

    //csv関連(using System & using System.IOも追加)
    private string csvFilePath = "atk";
    private string[] csvDatas = new string[] { };

    public AttackCard()
    {
        this.atk_c = 0;
        this.atk_i = 0;
        this.atk_a = 0;
    }

    public AttackCard(string id, string name, string attribute, string describe, int c, int i, int a, string illust)
    {
        this.card_id = id;
        this.card_name = name;
        this.card_attribute = attribute;
        this.card_describe = describe;
        if (this.card_attribute.Equals("Confidentiality"))
        { // 機密性
            this.atk_c = c;
            this.atk_i = 0;
            this.atk_a = 0;
        }
        else if (this.card_attribute.Equals("Integrity"))
        { // 完全性
            this.atk_c = 0;
            this.atk_i = i;
            this.atk_a = 0;
        }
        else if (this.card_attribute.Equals("Availability"))
        { // 可用性
            this.atk_c = 0;
            this.atk_i = 0;
            this.atk_a = a;
        }
        else if (this.card_attribute.Equals("Overall"))
        { // 全属性
            this.atk_c = c;
            this.atk_i = i;
            this.atk_a = a;
        }
        this.card_color = Color.red;
        this.card_illust = illust;
    }

    //IDに対応した数字からcsvを読み込むコンストラクタ
    public AttackCard(int num){
        CsvReader csvReader = new CsvReader();
        csvDatas = csvReader.Readcsv(csvFilePath,num);

        this.card_id = csvDatas[0];
        this.card_name = csvDatas[1];
        this.card_attribute = csvDatas[2];
        this.card_describe = csvDatas[3];
        this.atk_c = Convert.ToInt32(csvDatas[4]);
        if (this.card_attribute.Equals("Confidentiality"))
        { // 機密性
            this.atk_c = Convert.ToInt32(csvDatas[4]);
            this.atk_i = 0;
            this.atk_a = 0;
        }
        else if (this.card_attribute.Equals("Integrity"))
        { // 完全性
            this.atk_c = 0;
            this.atk_i = Convert.ToInt32(csvDatas[4]);
            this.atk_a = 0;
        }
        else if (this.card_attribute.Equals("Availability"))
        { // 可用性
            this.atk_c = 0;
            this.atk_i = 0;
            this.atk_a = Convert.ToInt32(csvDatas[4]);
        }
        else if (this.card_attribute.Equals("Overall"))
        { // 全属性
            this.atk_c = Convert.ToInt32(csvDatas[4]);
            this.atk_i = Convert.ToInt32(csvDatas[4]);
            this.atk_a = Convert.ToInt32(csvDatas[4]);
        }
        this.card_color = Color.red;
        // 現時点でatk.csvに無いのでとりあえず5番目に
        this.card_illust = csvDatas[5];
    }

	// Use this for initialization
	void Start ()
    {
        
    }
	
	// Update is called once per frame
	void Update ()
    {
		
	}

}
