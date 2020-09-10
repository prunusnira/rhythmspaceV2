using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RecordData {
    private int id;
    private string name;
    private string rank;
    private int score;
    private string md5hash;

    public RecordData() { }

    public RecordData(int id, string name, string rank, int score, string md5hash)
    {
        SetId(id);
        SetName(name);
        SetRank(rank);
        SetScore(score);
        SetMD5(md5hash);
    }

    public int GetId()
    {
        return id;
    }
    public void SetId(int id)
    {
        this.id = id;
    }
    public string GetName()
    {
        return name;
    }
    public void SetName(string name)
    {
        this.name = name;
    }
    public string GetRank()
    {
        return rank;
    }
    public void SetRank(string rank)
    {
        this.rank = rank;
    }
    public int GetScore()
    {
        return score;
    }
    public void SetScore(int score)
    {
        this.score = score;
    }
    public string GetMD5()
    {
        return md5hash;
    }
    public void SetMD5(string md5hash)
    {
        this.md5hash = md5hash;
    }
}
