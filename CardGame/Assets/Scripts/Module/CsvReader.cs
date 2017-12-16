using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class CsvReader{

    //メンバ変数はどれが事前に必要なのか不明なのでTextAssetだけにしました
    //変数と関数は必要そうなら順次増やしてってください
    //csvから読み込むカードの画像のパスも暫定でstring想定です

    public TextAsset csvFile;

    public CsvReader() { }

    public string[] Readcsv(string csvFilePath ,int num)
    {
        csvFile = Resources.Load("CSV/" + csvFilePath) as TextAsset;
        StringReader reader = new StringReader(csvFile.text);

        for (int i = 0; i < num; i++)
        {
            reader.ReadLine();
        }

        string line = reader.ReadLine();
        string[] csvDatas = line.Split(',');

        return csvDatas;
    }
}
