import { MapDifficulty } from "./Enums/Battle";

export function MapDifficultyText(Difficulty: MapDifficulty): string {
	switch(Difficulty){
		case MapDifficulty.Casual: return "diff.Casual";
		case MapDifficulty.Easy: return "diff.Easy";
		case MapDifficulty.Normal: return "diff.Normal";
		case MapDifficulty.Hard: return "diff.Hard";
	}
	return "";
}