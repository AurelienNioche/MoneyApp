using System;

/* 
WU: Wait for user action
WS: Wait for server reply
*/

namespace AssemblyCSharp
{
	public enum TL {
		
		HomeWU,
		InitWS,
		SurveyWU,
		SurveyWS,
		TutorialWU,
		TrainingStartWU,
		TrainingChoiceWU,
		TrainingChoiceWS,
		TrainingResultWU,
		TrainingEndWU,
		TrainingReadyWU,
		TrainingDoneWS,
		GameChoiceWU,
		GameChoiceWS,
		GameResultWU,
		End
	}
}

