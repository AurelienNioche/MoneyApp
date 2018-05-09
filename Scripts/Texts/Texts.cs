using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using AssemblyCSharp;

public class Texts {

	public static string goods = "Il était une fois un monde où il n'existait que {0} biens.";
	public static string specialization = "Chaque personne produisait un de ces biens mais en désirait un autre.";
	public static string specWood = "Ainsi, certaines produisaient du bois et désiraient du blé.";
	public static string specWheat = "Certaines produisaient du blé et désiraient de la pierre.";
	public static string specStone3G = "Certaines autres encore produisaient de la pierre et désiraient du bois.";
	public static string specStone4G = "Certaines produisaient de la pierre et désiraient de l'argile.";
	public static string specClay = "Certaines autres encore produisaient de l'argile et désiraient du bois.";
	public static string noCoincidenceP1 = "Notez que dans ce monde, les besoins n'étaient jamais symétriques :\n" +
		"si quelqu'un produisait du bois et désirait du blé...";
	public static string noCoincidenceP2 = "...il n'existait par exemple personne qui produisait du blé et désirait du bois.";
	public static string exchange = "Pour obtenir le bien souhaité, \nchaque personne était obligée de procéder à une suite d'échanges.";
	public static string you = "Vous allez incarner {0}, \nun producteur de bois à la recherche de blé.";
	public static string directStrategy = "Pour obtenir du blé, vous aurez deux stratégies possibles : " +
		"tenter d'échanger votre bois contre du blé directement... ";
	public static string indirectStrategy3G = "...ou bien échanger dans un premier temps votre bois contre de la pierre, " +
		"puis dans un second temps échanger la pierre obtenue contre du blé.";
	public static string indirectStrategy4G = "...ou bien échanger dans un premier temps votre bois contre de la pierre ou de l'argile, " +
		"puis dans un second temps échanger la pierre ou l'argile obtenue contre du blé.";
	public static string production = "A chaque fois que vous obtiendrez du blé, \nvous gagnerez un point et\n" +
		"vous produirez une nouvelle unité de bois.";

	// Training
	public static string training = "Commençons par une partie d'entrainement contre des joueurs virtuels !";
	public static string ready = "Vous êtes arrivés au terme du tutoriel. \nPrêts à jouer ?";

	// Waiting
	public static string waitingOtherPlayers = "En attente des autres joueurs...";

	public static string consent = "Vous déclarez consentir à ce que les données recueillies dans le cadre de cette étude soient enregistrées, " +
	                               "traitées et publiées sous une forme anonyme, " +
	                               "que vous avez été informé de la procédure de l'étude et que vous participez " +
	                               "de votre plein gré à l'étude mentionnée.";
}

public class Title {
	
	public static string survey = "Prélude";
	public static string tutorial = "Prélude";
	public static string end = "Fin";
	public static string training = "Entrainement";
	public static string title = "Ganomics";
}

public class ErrorMsg {
	
	public static string age = "Vous devez indiquer votre âge !";
	public static string sex = "Vous devez indiquer votre sexe !";
	public static string consent = "Vous devez donner votre consentement !";
}
