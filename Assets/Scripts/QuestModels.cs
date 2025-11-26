using System;
using System.Collections.Generic;

[Serializable]
public class QuestDTO
{
    public int id;
    public List<QueteNiveauAtteint> queteNiveauAtteints;
    public List<QueteVaincreMonstres> quetesVaincreMonstre;
    public List<QueteVisiterTuile> quetesVisiterTuile;

    public int nbQuetes;
    public int nbQuetesMAX;
}

[Serializable]
public class QueteNiveauAtteint
{
    public int id;
    public int niveauAAtteindre;
    public int idPersonnage;
    public int niveauPerso;
    public bool isActive;
}

[Serializable]
public class QueteVaincreMonstres
{
    public int id;
    public int nbMonstresAVaincre;
    public string typeMonstre;
    public int idPersonnage;
    public int nbMonstresVaincu;
    public bool isActive;
}

[Serializable]
public class QueteVisiterTuile
{
    public int id;
    public int x;
    public int y;
    public int distanceX;
    public int distanceY;
    public int idPersonnage;
    public bool isActive;
}
