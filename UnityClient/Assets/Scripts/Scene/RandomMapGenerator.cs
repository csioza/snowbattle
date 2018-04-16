using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/*
 * Usage:
 * rmg = new RandomMapGenerator ();
 * string json = rmg.generate ();
 */

public class RandomMapGenerator {

	enum Direction {
		NORTH,
		EAST,
		SOUTH,
		WEST,
		COUNT
	};
	float[] directionRatio;
	int mainlineLength;
	int roomNum;

	int[] roomIds;
	List<GenGate> branchableGates;
	//int[] numOfProps;

	List<Rect> occupiedArea;

	List<GenVariable> variablesList;
    List<GenEvents> eventsList;
    List<GenEResults> eresultList;
	int variablesCounter;

	int roomIdCounter;

	//GenLevel currentWorkingLevel;
	int maxMonsterNum;
	int[] monsterNums;
    int monstersettingID = 2;//8;//31;//8;
	//int[] levelCount;

	public RandomMapGenerator() {
		// TODO: these are some hard code param, maybe change to sth editable on app....?
		directionRatio = new float[(int)Direction.COUNT];
		directionRatio [(int)Direction.NORTH] = 4f;
		directionRatio [(int)Direction.EAST] = 6f;
		directionRatio [(int)Direction.SOUTH] = 6f;
		directionRatio [(int)Direction.WEST] = 4f;
		
		mainlineLength = 7;
		roomNum = 9;

		// some other hardcode
		roomIds = new int[] {1, 1, 1};
		//numOfProps = new int[] {0, 2, 5};

		maxMonsterNum = 5;
		monsterNums = new int[maxMonsterNum];
		for(int i=0;i<maxMonsterNum;i++) {
			monsterNums[i] = i+1;
		}

		//levelCount = new int[]{1,2,3};
	}

	// 修改預設的隨機地圖生成條件數值
	public void setup(
			int roomNum, 				// 樓層中的房間數目
			int mainlineLength, 		// 主線房間數目，即由起始房間前往完結房間／傳送門房間之間的房間數目（包括起始和完結房間）（必須相等或少於樓層中的房間數目）
			float directionRatioN, 		// 房間朝向北方發展的比重
			float directionRatioE, 		// 房間朝向東方發展的比重
			float directionRatioS, 		// 房間朝向南方發展的比重
			float directionRatioW, 		// 房間朝向西方發展的比重
			int maxMonsterNum) {		// 每間房間怪物數目上限
		
		directionRatio = new float[(int)Direction.COUNT];
		directionRatio [(int)Direction.NORTH] = directionRatioN;
		directionRatio [(int)Direction.EAST] = directionRatioE;
		directionRatio [(int)Direction.SOUTH] = directionRatioS;
		directionRatio [(int)Direction.WEST] = directionRatioW;
		
		this.mainlineLength = mainlineLength;
		this.roomNum = roomNum;
		
		this.maxMonsterNum = maxMonsterNum;
		monsterNums = new int[maxMonsterNum];
		for(int i=0;i<maxMonsterNum;i++) {
			monsterNums[i] = i+1;
		}
	}

	public string generate() {
		return generateObj().jsonEncode();
	}

	public GenFloor generateObj() {
		int x = 0;
		int y = 0;

//		Random.seed = System.Guid.NewGuid ().GetHashCode ();
		
		// TODO: (defensive) first check any required variables un-init
		
		GenFloor floor = new GenFloor ();
		variablesList = new List<GenVariable> ();
		variablesCounter = 1;
        eventsList = new List<GenEvents>();
        eresultList = new List<GenEResults>();
		
//		int levelNum = randomGen (levelCount);
		int levelNum = 1;
		floor.levels = new GenLevel[levelNum];

		for(int levelNo = 1; levelNo <= levelNum; levelNo++) 
        {
			branchableGates = new List<GenGate> ();
			occupiedArea = new List<Rect> ();

			GenLevel level = new GenLevel ();
//			currentWorkingLevel = level;
			//level.events = new List<GenEvent> ();
			level.levelId = levelNo;

			int mainline = mainlineLength;
			
			// mainline generating
			level.startingRoom = genNextRoom (x, y, true, (levelNo == levelNum), mainline);

			// branch generating
			for (int i=0; i<roomNum-mainline; i++) {
				GenRoom newRoom;
				GenBridge newBridge;
				GenGate selectedGate;

				do {
					if (branchableGates.Count == 0)
					{
						selectedGate = null;
						newRoom = null;
						newBridge = null;
						break;
					}
					int randomIndex = Random.Range(0, branchableGates.Count-1);
					selectedGate = branchableGates[randomIndex];
					newBridge = genBridge(selectedGate.x, selectedGate.y, selectedGate.direction);
					newRoom = genNextRoom(newBridge.x, newBridge.y, false, false, 0, (selectedGate.direction < 2 ? selectedGate.direction + 2 : selectedGate.direction - 2));
					if (newRoom == null)
					{
						branchableGates.RemoveAt(randomIndex);
					}
				} while(newRoom == null);
				if (selectedGate != null && newRoom != null && newBridge != null)
				{
					branchableGates.Remove(selectedGate);
					selectedGate.isGateOpen = true;
					selectedGate.bridge = newBridge;
					selectedGate.bridge.targetRoom = newRoom;

					genGateEvent(newRoom, selectedGate.direction);
				}				
			}
            //mark by gpf
// 			if(levelNo == levelNum) 
//             {
// 				// Boss room generate
// 				level.bossRoom = genNextRoom(1000, 1000, false);
// 				level.bossRoom.monsters = new GenMonster[1];
// 				// get a position for boss
// 				// TODO: stop using silly method
// 				Room roomData = new Room (level.bossRoom.roomObjId);
// 				bool validPos = false;
// 				Pos position = new Pos(0, 0);
// 				while(!validPos) {
// 					position.x = Random.Range (0, 13);
// 					position.y = Random.Range (0, 13);
// 					if(roomData.landscape[position.y][position.x] == 1) {
// 						validPos = true;
// 					}
// 				}
// 				roomData.landscape[position.y][position.x] = 0;
// 				level.bossRoom.monsters[0] = genMonster(0, position);
// 				level.bossRoom.monsters[0].appearCondId = 0;
// 				level.bossRoom.monsters [0].monsterObjId = 2;
// 				// make the event that when the boss is killed, player wins
// //				level.bossRoom.monsters[0].events = new List<GenEvent>();
// // 				GenEvent newGE = new GenEvent();
// // 				newGE.triggerTypeId = GenEvent.TriggerType.MONSTER_DIE;
// // 				newGE.results = new GenEventResult[1];
// // 				GenEventResult newGER = new GenEventResult();
// // 				newGER.resultTypeId = GenEvent.ResultType.FLOOR_CLEAR;
// // 				newGE.results[0] = newGER;
// // 				level.bossRoom.monsters[0].events.Add(newGE);
// 
            //gen event added by kone
            //GenEvents genEvent = new GenEvents();
            //genEvent.eventId = 1;
            //genEvent.triggerTypeId = GenEvents.TriggerType.MONSTER_DIE;
            //genEvent.levelId = level.levelId;
            //genEvent.roomId = level.bossRoom.roomId;
            //genEvent.triggerParams = new int[1];
            //genEvent.triggerParams[0] = level.bossRoom.monsters[0].monsterId;
            //eventsList.Add(genEvent);
            //GenEResults genEResult = new GenEResults();
            //genEResult.resultTypeId = GenEResults.ResultType.FLOOR_CLEAR;
            //genEResult.eventId = 1;
            //eresultList.Add(genEResult);
// 				// get char Position
// 				validPos = false;
// 				position = new Pos(0, 0);
// 				while(!validPos) {
// 					position.x = Random.Range (0, 13);
// 					position.y = Random.Range (0, 13);
// 					if(roomData.landscape[position.y][position.x] == 1) {
// 						validPos = true;
// 					}
// 				}
// 				level.bossRoom.charPosition = position;
// 				roomData.landscape[position.y][position.x] = 0;
// 			}
			floor.levels [levelNo-1] = level;
		}
		
		floor.monsters = new GenMonsterObj[3];
		// normal monster
		floor.monsters [0] = new GenMonsterObj ();
		floor.monsters [0].monsterObjId = 0;
        floor.monsters[0].monsterSettingId = monstersettingID;
		// stronger monster
		floor.monsters [1] = new GenMonsterObj ();
		floor.monsters [1].monsterObjId = 1;
        floor.monsters[1].monsterSettingId = monstersettingID;
		floor.monsters [1].skills = new GenSkill[2];
		floor.monsters [1].skills [0] = new GenSkill ();
		floor.monsters [1].skills [0].skillId = 1;
		floor.monsters [1].skills [1] = new GenSkill ();
		floor.monsters [1].skills [1].skillId = 2;
		floor.monsters [1].skills [1].skillParam = new int[2];
		floor.monsters [1].skills [1].skillParam[0] = 150;
		floor.monsters [1].skills [1].skillParam[1] = 5;
		// boss
		floor.monsters [2] = new GenMonsterObj ();
		floor.monsters [2].monsterObjId = 2;
        floor.monsters[2].monsterSettingId = monstersettingID;
		floor.monsters [2].skills = new GenSkill[2];
		floor.monsters [2].skills [0] = new GenSkill ();
		floor.monsters [2].skills [0].skillId = 1;
		floor.monsters [2].skills [1] = new GenSkill ();
		floor.monsters [2].skills [1].skillId = 2;
		floor.monsters [2].skills [1].skillParam = new int[1];
		floor.monsters [2].skills [1].skillParam[0] = 220;

		//floor.variables = variablesList.ToArray ();
        floor.events = eventsList.ToArray();
        floor.eresults = eresultList.ToArray();
        
		return floor;
	}

	public GenRoom genNextRoom(int x, int y, bool isMainline, bool isLastLevel = false, int length = 0, int incomeDirection = -1) {
		GenRoom newRoom = new GenRoom ();
		newRoom.roomObjId = randomGen (roomIds);
		Room roomData = new Room (newRoom.roomObjId);
		newRoom.roomId = roomIdCounter++;
		int monsterNum = 0;

		// first calibrate the position
		if(incomeDirection != -1) {
			x -= roomData.gates [incomeDirection].pos.x;
			y -= roomData.gates [incomeDirection].pos.y;
		}else {
			x -= 8;
			y -= 8;
		}
		// check collision
		Rect newArea = new Rect (x, y, 16, 16);
		if (collisionCheck (newArea)) {
			return null;
		}
		occupiedArea.Add (newArea);
		
		// add monster to room
		// check if needed (not starting room)
		if(length == mainlineLength) {
			// starting room need character starting position
			//bool validPos = false;
			newRoom.charPosition = new Pos(5, 5);
// 			while(!validPos) {
// 				newRoom.charPosition.x = Random.Range (0, 13);
// 				newRoom.charPosition.y = Random.Range (0, 13);
// 				if(roomData.landscape[newRoom.charPosition.y][newRoom.charPosition.x] == 1) {
// 					validPos = true;
// 				}
// 			}
		} else if(length == 1) {
			// last room, let this be the teleport gate room
			newRoom.teleportGate = new GenTGate();
			newRoom.teleportGate.teleportGateObjId = 1;
			bool validPos = false;
			newRoom.teleportGate.position = new Pos(0, 0);
			while(!validPos) {
				newRoom.teleportGate.position.x = Random.Range (0, 13);
				newRoom.teleportGate.position.y = Random.Range (0, 13);
				if(roomData.landscape[newRoom.teleportGate.position.y][newRoom.teleportGate.position.x] == 1) {
					validPos = true;
				}
			}
			if(isLastLevel) {
				newRoom.teleportGate.target = "boss";
			} else {
				newRoom.teleportGate.target = "level";
			}
		} 
       // else {
			// random number of monsters
            monsterNum = maxMonsterNum;// randomGen(monsterNums);
			newRoom.monsters = new GenMonster[monsterNum];
			for(int i=0;i<monsterNum;i++) {
				// get a position
				// TODO: stop using silly method
				bool validPos = false;
				Pos position = new Pos(0, 0);
				
				while(!validPos) {
					position.x = Random.Range (0, 13);
					position.y = Random.Range (0, 13);
					if(roomData.landscape[position.y][position.x] == 1) {
						// mark area around nolonger avaliable
						roomData.landscape[position.y-1][position.x] = 0;
						roomData.landscape[position.y+1][position.x] = 0;
						roomData.landscape[position.y][position.x-1] = 0;
						roomData.landscape[position.y][position.x+1] = 0;
						roomData.landscape[position.y][position.x] = 0;
						
						validPos = true;
					}
				}
				newRoom.monsters[i] = genMonster(i, position);
			}
		//}

		// mark all gate position
		for(int i=0;i<4;i++) {
			newRoom.gates[i].x = x + roomData.gates[i].pos.x;
			newRoom.gates[i].y = y + roomData.gates[i].pos.y;
		}

		if (isMainline) {
			// get the next room (recursion), attach to the bridge
			if(length > 1) {
				// random a gate
				int direction;

				GenBridge newBridge = null;
				GenRoom nextRoom = null;
				int[] directions = new int[]{0, 1, 2, 3};
				List<int> directionList = new List<int>(directions);
				List<float> ratioList = new List<float>(directionRatio);
				if (directionList.Remove(incomeDirection) == true)
				{
					ratioList.RemoveAt(incomeDirection);
				}				
				do{
					if (directionList.Count == 0)
					{
						return null;
					}

					direction = randomGen(directionList.ToArray(), ratioList.ToArray());
					int directionIndex = directionList.IndexOf(direction);
					directionList.RemoveAt(directionIndex);
					ratioList.RemoveAt(directionIndex);

					newBridge = genBridge(newRoom.gates[direction].x, newRoom.gates[direction].y, direction);
					nextRoom = genNextRoom(newBridge.x, newBridge.y, isMainline, isLastLevel, length - 1, (direction<2?direction+2:direction-2));
				} while (nextRoom == null) ;
				
				// open the gate
				newRoom.gates[direction].isGateOpen = true;

				int gateOpenMethod = genGateEvent(newRoom, direction);


				// TODO: Old open gate method, get ride of this when have time to implement event system on client side
				newRoom.gates[direction].gateOpenCondId = gateOpenMethod;
				if(newRoom.monsters == null) {
					newRoom.gates[direction].gateOpenCondId = 0;
				} else if(newRoom.gates[direction].gateOpenCondId == 2) {
					// need to kill specific monster, mark which one
					// temp all monsterObj = 1
					List<int> targetMonsters = new List<int>();
					for(int i=0;i<monsterNum;i++) {
						if(newRoom.monsters[i].monsterObjId == 1) {
							targetMonsters.Add(i);
						}
					}
					newRoom.gates[direction].gateOpenCondParam = targetMonsters.ToArray();
				}

				// get a bridge and connect to the gate
				newRoom.gates[direction].bridge = newBridge;
				newRoom.gates[direction].bridge.targetRoom = nextRoom;
				
				// gather list of unused gate for branching
				if(length != mainlineLength) {
					for(int i=0;i<4;i++) {
						if(i != direction && i != incomeDirection) {
							branchableGates.Add (newRoom.gates[i]);
						}
					}
				}
			}
		} else {
			
		}

		return newRoom;
	}
	
	public GenBridge genBridge(int x, int y, int direction) {
		GenBridge gb = new GenBridge ();
		gb.bridgeCenterLength = Random.Range (1, 4);

		Pos p = new Pos (0, 0);
		switch(direction) {
		case 0:
			p.y = -1;
			break;
		case 1:
			p.x = 1;
			break;
		case 2:
			p.y = 1;
			break;
		case 3:
			p.x = -1;
			break;
		}

		// each bridge component equal to 2 square, so bridge head + bridge middle * length + bridge end = ....
		gb.x = x + (1 + gb.bridgeCenterLength + 1) * 2 * p.x;
		gb.y = y + (1 + gb.bridgeCenterLength + 1) * 2 * p.y;

		return gb;
	}

	public int genGateEvent(GenRoom newRoom, int direction) {
		// prepare event to open the gate
		// before everything start, lets check if any monsters
		int gateOpenMethod;
		List<GenMonster> targetMonsters = new List<GenMonster>();
		if(newRoom.monsters == null) {
			gateOpenMethod = 0;
		
		// TODO: add chance to trigger/treasure open gate event here (or not here? need a better flow)
		} else {
			// lets check how many target monster here
			for(int i=0;i<newRoom.monsters.Length;i++) {
				// lets assume monsterObjId 1 as target monster
				if(newRoom.monsters[i].monsterObjId == 1) {
					targetMonsters.Add (newRoom.monsters[i]);
				}
			}
			// currently there should be three type of method to open gate
			gateOpenMethod = randomGen(new int[]{0, 1});
			if(targetMonsters.Count > 0) {
				// kill specific monsters can only be done when target monsters exists
				gateOpenMethod = randomGen(new int[]{1, 2});
			}
		}
		switch(gateOpenMethod) {

		case 0:
		{
			// if no monster, gate must be opened already, so add an event on level start
// 			GenEvent gateOpeningEvent = new GenEvent();
// 			gateOpeningEvent.triggerTypeId = GenEvent.TriggerType.LEVEL_ENTER;
// 			gateOpeningEvent.results = new GenEventResult[1];
// 			gateOpeningEvent.results[0] = new GenEventResult();
// 			gateOpeningEvent.results[0].resultTypeId = GenEvent.ResultType.GATE_OPEN;
// 			gateOpeningEvent.results[0].resultParams = new float[]{currentWorkingLevel.levelId, newRoom.roomId, direction};
// 			
// 			currentWorkingLevel.events.Add(gateOpeningEvent);
		}
			break;
			// 1) kill all monsters
			// Event setting: all monster will add 1 to a global variable when being killed
			//					when that global variable reach the number of monsters, trigger an event to open the gate
		case 1:
		{
			// 1. set monster event, when die, add a counter
			GenVariable newVar = new GenVariable();
			newVar.variableId = variablesCounter++;
			newVar.initValue = 0;
			variablesList.Add(newVar);
			
			// 2. Create an event that, when a monster dead, add up a counter
// 			GenEvent monsterDieEvent = new GenEvent();
// 			monsterDieEvent.triggerTypeId = GenEvent.TriggerType.MONSTER_DIE;
// 			monsterDieEvent.results = new GenEventResult[1];
// 			monsterDieEvent.results[0] = new GenEventResult();
// 			monsterDieEvent.results[0].resultTypeId = GenEvent.ResultType.VAR_SET_AS_VAR_PLUS_NUM;
// 			monsterDieEvent.results[0].resultParams = new float[]{newVar.variableId, newVar.variableId, 1f};
// 			
// 			// 3. Create an event that, when a monster dead, check if thee counter is larger than or equal to room monster num, if yes then open the gate
// 			GenEvent gateOpeningEvent = new GenEvent();
// 			gateOpeningEvent.triggerTypeId = GenEvent.TriggerType.MONSTER_DIE;
// 			gateOpeningEvent.conds = new GenEventCond[1];
// 			gateOpeningEvent.conds[0] = new GenEventCond();
// 			gateOpeningEvent.conds[0].condTypeId = GenEvent.CondType.LOGIC_LARGER_OR_EQUAL_NUM;
// 			gateOpeningEvent.conds[0].condParams = new float[]{newVar.variableId, newRoom.monsters.Length};
// 			gateOpeningEvent.results = new GenEventResult[1];
// 			gateOpeningEvent.results[0] = new GenEventResult();
// 			gateOpeningEvent.results[0].resultTypeId = GenEvent.ResultType.GATE_OPEN;
// 			gateOpeningEvent.results[0].resultParams = new float[]{currentWorkingLevel.levelId, newRoom.roomId, direction};
// 			
// 			// 4. now assign both event to every monster in room
// 			for(int i=0;i<newRoom.monsters.Length;i++) {
// 				if(newRoom.monsters[i].events == null) {
// 					newRoom.monsters[i].events = new List<GenEvent>();
// 				}
// 				newRoom.monsters[i].events.Add(monsterDieEvent);
// 				newRoom.monsters[i].events.Add (gateOpeningEvent);
// 			}
		}
			break;
			
			// 2) kill specific monsters
			// Event setting: similar to above, be the counter variable is not all monsters, 
			//					and not all monsters have the triggering event
		case 2:
		{
			// 1. set monster event, when die, add a counter
			GenVariable newVar = new GenVariable();
			newVar.variableId = variablesCounter++;
			newVar.initValue = 0;
			
			
			// 2. Create an event that, when a monster dead, add up a counter
// 			GenEvent monsterDieEvent = new GenEvent();
// 			monsterDieEvent.triggerTypeId = GenEvent.TriggerType.MONSTER_DIE;
// 			monsterDieEvent.results = new GenEventResult[1];
// 			monsterDieEvent.results[0] = new GenEventResult();
// 			monsterDieEvent.results[0].resultTypeId = GenEvent.ResultType.VAR_SET_AS_VAR_PLUS_NUM;
// 			monsterDieEvent.results[0].resultParams = new float[]{newVar.variableId, newVar.variableId, 1f};
// 			
// 			// 3. Create an event that, when a monster dead, check if thee counter is larger than or equal to room monster num, if yes then open the gate
// 			GenEvent gateOpeningEvent = new GenEvent();
// 			gateOpeningEvent.triggerTypeId = GenEvent.TriggerType.MONSTER_DIE;
// 			gateOpeningEvent.conds = new GenEventCond[1];
// 			gateOpeningEvent.conds[0] = new GenEventCond();
// 			gateOpeningEvent.conds[0].condTypeId = GenEvent.CondType.LOGIC_LARGER_OR_EQUAL_NUM;
// 			gateOpeningEvent.conds[0].condParams = new float[]{newVar.variableId, targetMonsters.Count};
// 			gateOpeningEvent.results = new GenEventResult[1];
// 			gateOpeningEvent.results[0] = new GenEventResult();
// 			gateOpeningEvent.results[0].resultTypeId = GenEvent.ResultType.GATE_OPEN;
// 			gateOpeningEvent.results[0].resultParams = new float[]{currentWorkingLevel.levelId, newRoom.roomId, direction};
// 			
// 			// 4. now assign both event to every monster in room
// 			for(int i=0;i<targetMonsters.Count;i++) {
// 				if(targetMonsters[i].events == null) {
// 					targetMonsters[i].events = new List<GenEvent>();
// 				}
// 				targetMonsters[i].events.Add(monsterDieEvent);
// 				targetMonsters[i].events.Add (gateOpeningEvent);
// 			}
		}
			break;
			
			// 3) use key
			// Event setting: TODO <--- seems need to expand data structure
			
		}

		return gateOpenMethod;
	}
	
	public GenMonster genMonster(int mid, Pos pos) {
		GenMonster monster = new GenMonster ();
		monster.monsterId = mid;
		monster.monsterObjId = randomGen(new int[]{0, 1});
		monster.position = pos;
//		monster.dropKeyId = -1;
		monster.appearCondId = 1;

		return monster;
	}

	public T randomGen<T>(T[] items, float[] weight = null) {
		float maxScore = 0;

		if (weight == null) {
			weight = new float[items.Length];
			for(int i = 0; i < items.Length; i++) {
				weight[i] = 1f;
			}
		}

		foreach (float score in weight) {
			maxScore += score;
		}

		float rnd = Random.Range (0f, maxScore);

		for (int i = 0; i < weight.Length; i++) {
			rnd -= weight[i];
			if(rnd < 0) {
				return items[i];
			}
		}

		return items[items.Length - 1];
	}

	public bool collisionCheck(Rect newArea) {
		foreach(Rect area in occupiedArea) {
			if(area.Overlaps(newArea)) {
				return true;
			}
		}
		return false;
	}
}

public class Room {
	public int[][] landscape;
	public Gate[] gates;

	public Room(int id = 0) {
		// temp hard code.....
		switch (id) {
		case 1:
			landscape = new int[14][];
			landscape [13] = new int[] {0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0};
			landscape [12] = new int[] {0, 0, 0, 0, 0, 1, 0, 0, 0, 0, 0, 0, 0, 0};
			landscape [11] = new int[] {0, 0, 0, 0, 0, 1, 1, 1, 1, 1, 0, 0, 0, 0};
			landscape [10] = new int[] {0, 0, 0, 0, 0, 1, 1, 1, 1, 1, 1, 0, 0, 0};
			landscape [9]  = new int[] {0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 1, 0, 0, 0};
			landscape [8]  = new int[] {0, 0, 0, 0, 0, 0, 0, 1, 1, 1, 1, 1, 0, 0};
			landscape [7]  = new int[] {0, 0, 0, 0, 0, 0, 0, 1, 1, 1, 1, 1, 0, 0};
			landscape [6]  = new int[] {0, 0, 0, 0, 0, 0, 0, 1, 1, 0, 0, 0, 0, 0};
			landscape [5]  = new int[] {0, 0, 0, 0, 0, 0, 1, 1, 1, 0, 0, 0, 0, 0};
			landscape [4]  = new int[] {0, 0, 0, 0, 1, 1, 1, 1, 1, 0, 0, 0, 0, 0};
			landscape [3]  = new int[] {0, 1, 1, 1, 0, 0, 1, 1, 1, 1, 1, 0, 0, 0};
			landscape [2]  = new int[] {0, 0, 0, 0, 0, 0, 1, 1, 1, 1, 1, 0, 0, 0};
			landscape [1]  = new int[] {0, 0, 0, 0, 0, 0, 1, 1, 1, 0, 0, 0, 0, 0};
			landscape [0]  = new int[] {0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0};
			
			gates = new Gate[4];
			gates[0] = new Gate(new Pos(3, 	0));
			gates[1] = new Gate(new Pos(13, 8));
			gates[2] = new Gate(new Pos(5, 	13));
			gates[3] = new Gate(new Pos(0, 	8));
			break;
		case 2:
			landscape = new int[14][];
			landscape [13] = new int[] {0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0};
			landscape [12] = new int[] {0, 0, 0, 0, 0, 0, 1, 1, 1, 1, 0, 0, 0, 0};
			landscape [11] = new int[] {0, 0, 0, 0, 0, 0, 1, 1, 1, 1, 1, 0, 0, 0};
			landscape [10] = new int[] {0, 0, 0, 0, 0, 0, 1, 1, 1, 1, 1, 1, 0, 0};
			landscape [9]  = new int[] {0, 0, 0, 0, 1, 1, 1, 1, 1, 0, 0, 0, 0, 0};
			landscape [8]  = new int[] {0, 0, 0, 0, 1, 0, 1, 0, 1, 0, 0, 0, 0, 0};
			landscape [7]  = new int[] {0, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 0, 0};
			landscape [6]  = new int[] {0, 1, 1, 1, 1, 0, 0, 1, 1, 1, 1, 1, 0, 0};
			landscape [5]  = new int[] {0, 1, 1, 1, 1, 0, 0, 0, 0, 1, 1, 1, 1, 0};
			landscape [4]  = new int[] {0, 1, 1, 1, 1, 0, 0, 0, 0, 1, 1, 1, 0, 0};
			landscape [3]  = new int[] {0, 0, 1, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0};
			landscape [2]  = new int[] {0, 1, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0};
			landscape [1]  = new int[] {0, 0, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0};
			landscape [0]  = new int[] {0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0};
			
			gates = new Gate[4];
			gates[0] = new Gate(new Pos(3, 	0));
			gates[1] = new Gate(new Pos(13, 8));
			gates[2] = new Gate(new Pos(5, 	13));
			gates[3] = new Gate(new Pos(0, 	8));
			break;
		case 3:
			landscape = new int[14][];
			landscape [13] = new int[] {0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0};
			landscape [12] = new int[] {0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0};
			landscape [11] = new int[] {0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0};
			landscape [10] = new int[] {0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0};
			landscape [9]  = new int[] {0, 0, 1, 1, 1, 1, 1, 1, 0, 0, 0, 0, 0, 0};
			landscape [8]  = new int[] {0, 0, 1, 1, 1, 1, 1, 1, 1, 0, 0, 0, 0, 0};
			landscape [7]  = new int[] {0, 0, 0, 0, 0, 1, 1, 0, 0, 1, 1, 1, 1, 0};
			landscape [6]  = new int[] {0, 0, 0, 0, 0, 1, 1, 1, 1, 1, 1, 1, 1, 0};
			landscape [5]  = new int[] {0, 0, 0, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 0};
			landscape [4]  = new int[] {0, 0, 0, 1, 1, 1, 1, 1, 1, 1, 1, 1, 0, 0};
			landscape [3]  = new int[] {0, 0, 0, 0, 0, 1, 1, 1, 0, 0, 0, 0, 0, 0};
			landscape [2]  = new int[] {0, 0, 0, 0, 0, 1, 1, 1, 0, 0, 0, 0, 0, 0};
			landscape [1]  = new int[] {0, 0, 0, 0, 0, 1, 1, 1, 0, 0, 0, 0, 0, 0};
			landscape [0]  = new int[] {0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0};
			
			gates = new Gate[4];
			gates[0] = new Gate(new Pos(3, 	0));
			gates[1] = new Gate(new Pos(13, 8));
			gates[2] = new Gate(new Pos(5, 	13));
			gates[3] = new Gate(new Pos(0, 	8));
			break;
		}
	}
}

public class Gate {
	public Pos pos;

	public Gate(Pos p) {
		pos = p;
	}
}

public class Bridge {

}

public class Pos {
	public int x;
	public int y;

	public Pos(int x, int y) {
		this.x = x;
		this.y = y;
	}

	public string jsonEncode() {
		string j;
		j = "{" +
				"\"x\": " + x + "," + 
				"\"y\": " + y + 
			"}";

		return j;
	}
}

public class GenFloor {
	public GenVariable[] variables;
	public GenMonsterObj[] monsters;
	public GenLevel[] levels;
    public GenEvents[] events;
    public GenEResults[] eresults;

	public string jsonEncode() {
// 		string jVariables = "";
// 		if(variables != null) {
// 			foreach(GenVariable variable in variables) {
// 				jVariables += variable.jsonEncode() + ",";
// 			}
// 		}
		string jMonsters = "";
		if(monsters != null) {
			foreach(GenMonsterObj monster in monsters) {
				jMonsters += monster.jsonEncode() + ",";
			}
		}
		string jLevels = "";
		foreach(GenLevel level in levels) {
			jLevels += level.jsonEncode() + ",";
		}
        string jevents = "";
        foreach (GenEvents eve in events)
        {
            jevents += eve.jsonEncode() + ",";
        }
        string jeresults = "";
        foreach (GenEResults eve in eresults)
        {
            jeresults += eve.jsonEncode() + ",";
        }
        
		string j;
		j = "\"floor\": {" + 
				//"\"variables\": [" + jVariables.TrimEnd(',') + "]," + 
				(jMonsters!=""?"\"monsters\": [" + jMonsters.TrimEnd(',')+ "],":"") +
                "\"levels\": [" + (jLevels != "" ? jLevels.TrimEnd(',') : "\"\"") + "]," +
                "\"events\": [" + (jevents != "" ? jevents.TrimEnd(',') : "\"\"") + "]," +
                "\"results\": [" + (jeresults != "" ? jeresults.TrimEnd(',') : "\"\"") + "]" + 
			"}" + ",";
		j = "{" + j.TrimEnd(',') + "}";
		return j;
	}
}

public class GenLevel {
	public int levelId;
	public GenRoom startingRoom;
	public GenRoom bossRoom;
	//public List<GenEvent> events;

	public string jsonEncode() {
// 		string jEvents = "";
// 		if(events != null) {
// 			foreach(GenEvent e in events) {
// 				jEvents += e.jsonEncode() + ",";
// 			}
// 		}

		string j;
        j = "\"levelId\": " + levelId.ToString() + "," +
            "\"startingRoom\": " + (startingRoom != null ? startingRoom.jsonEncode() : "\"\"") + "," +
            (bossRoom != null ? "\"bossRoom\": " + bossRoom.jsonEncode() + "," : "") +
            ",";
        //"\"events\": [" + jEvents.TrimEnd(',') + "],";
		j = "{" + j.TrimEnd(',') + "}";
		return j;
	}
}

public class GenRoom {
	public int roomId;
	public int roomObjId;
	public Pos charPosition;
	public GenTGate teleportGate;
	public GenGate[] gates = new GenGate[4];
	public GenMonster[] monsters;
	public GenProp[] props;
	public GenTreasure[] treasures;
	public GenTrigger[] triggers;
	public int winCondId = -1;
	public int[] winCondParam;

	public GenRoom () {
		for (int i = 0; i < gates.Length; i++) {
			gates[i] = new GenGate();
			gates[i].room = this;
			gates[i].direction = i;
		}
	}

	public string jsonEncode() {
		string jGates = "";
		foreach(GenGate gate in gates) {
			jGates += gate.jsonEncode() + ",";
		}
		string jMonsters = "";
		if(monsters != null) {
			foreach(GenMonster monster in monsters) {
				jMonsters += monster.jsonEncode() + ",";
			}
		}
		string jProps = "";
		if(props != null) {
			foreach(GenProp prop in props) {
				jProps += prop.jsonEncode() + ",";
			}
		}

//         j = "\"treasureId\": " + treasureId.ToString() + "," +
//             "\"treasureObjId\": " + treasureObjId.ToString() + "," +
//             "\"position\": " + position.jsonEncode() + ",";
//         j = "{" +
// 				"\"x\": " + x + "," + 
// 				"\"y\": " + y + 
// 			"}";

        string jTreasure = "";
        string jj = "\"treasureId\": 100, \"treasureObjId\": 7, \"position\":{\"x\": 3,\"y\": 3}";
        jj = "{" + jj.TrimEnd(',') + "}";
        //jTreasure += jj;
// 		if(treasures != null) {
// 			foreach(GenTreasure treasure in treasures) {
// 				jTreasure += treasure.jsonEncode() + ",";
// 			}
// 		}
		string jTrigger = "";
		if(triggers != null) {
			foreach(GenTrigger trigger in triggers) {
				jTrigger += trigger.jsonEncode() + ",";
			}
		}

		string j;
		j = "\"roomId\": " + roomId.ToString() + "," + 
			"\"roomObjId\": " + roomObjId.ToString() + "," + 
			(charPosition!=null?"\"charPosition\": " + charPosition.jsonEncode() + ",":"") + 
			"\"gates\": [" + 
				jGates.TrimEnd(',') + 
			"]," + 
			"\"monsters\": [" + 
				jMonsters.TrimEnd(',') + 
			"]" + "," + 
			"\"props\": [" + 
				jProps.TrimEnd(',') + 
			"]" + "," + 
			"\"treasures\": [" + 
				jTreasure.TrimEnd(',') + 
			"]" + "," + 
			"\"triggers\": [" + 
				jTrigger.TrimEnd(',') + 
			"]" + "," + 
			(winCondId!=-1?"\"winCondId\": " + winCondId.ToString() + ",":"") +
			(winCondParam!=null?"\"winCondParam\": [" + winCondParam.ToString() + "],":"") + 
			(teleportGate!=null?"\"teleportGate\": " + teleportGate.jsonEncode() + ",":"");
		j = "{" + j.TrimEnd(',') + "}";
		return j;
	}
}

public class GenGate {
    public int gateId = 1;
	public int gateObjId = 3;
	public bool isGateOpen;
	public GenBridge bridge;
	
	public int gateOpenCondId;
	public int[] gateOpenCondParam;
	public int[] gateOpenCondCount;

	// generator use
	public GenRoom room;
	public int x;
	public int y;
	public int direction;
	
//	public string jsonEncode() {
//		string j;
//		j = "\"gateObjId\": " + gateObjId.ToString () + "," + 
//			"\"isGateOpen\": " + (isGateOpen?"1":"0") + "," + 
//			"\"bridge\": " + (bridge!=null?bridge.jsonEncode():"\"\"") + ",";
//		j = "{" + j.TrimEnd(',') + "}";
//		return j;
//	}
	public string jsonEncode() {
		string jGateOpenCondParam = "";
		if(gateOpenCondParam != null) {
			foreach(int param in gateOpenCondParam) {
				jGateOpenCondParam += param.ToString() + ",";
			}
		}
		string jGateOpenCondCount = "";
		if(gateOpenCondCount != null) {
			foreach(int count in gateOpenCondCount) {
				jGateOpenCondCount += count.ToString() + ",";
			}
		}
		
		string j;
        j = "\"serNo\": " + gateId.ToString() + "," + 
            "\"gateObjId\": " + gateObjId.ToString () + "," + 
			"\"isGateOpen\": " + (isGateOpen?"1":"0") + "," + 
				(gateOpenCondId!=-1?"\"gateOpenCondId\": " + gateOpenCondId.ToString() + ",":"") +
				(gateOpenCondParam!=null?"\"gateOpenCondParam\": [" + jGateOpenCondParam.TrimEnd(',') + "],":"") + 
				(jGateOpenCondCount!=null?"\"gateOpenCondCount\": [" + jGateOpenCondCount.TrimEnd(',') + "],":"") + 
				"\"bridge\": " + (bridge!=null?bridge.jsonEncode():"\"\"") + ",";
		j = "{" + j.TrimEnd(',') + "}";
		return j;
	}
}

public class GenBridge {
	public int bridgeObjId = 3;
	public int bridgeCenterLength;
	public GenRoom targetRoom;

	// generator use
	public int x;
	public int y;
	
	public string jsonEncode() {
		string j;
		j = "\"bridgeObjId\": " + bridgeObjId.ToString() + "," + 
			"\"bridgeCenterLength\": " + bridgeCenterLength.ToString() + "," + 
			"\"targetRoom\": " + (targetRoom!=null?targetRoom.jsonEncode():"\"\"") + ",";
		j = "{" + j.TrimEnd(',') + "}";
		return j;
	}
}

public class GenMonster {
	public int monsterId;
	public int monsterObjId;
	public Pos position;
	public int appearCondId = 1;
	//public List<GenEvent> events;
	
	public string jsonEncode() {
// 		string jEvents = "";
// 		if(events != null) {
// 			foreach(GenEvent e in events) {
// 				jEvents += e.jsonEncode() + ",";
// 			}
// 		}

		string j;
        j = "\"monsterId\": " + monsterId.ToString() + "," +
            "\"monsterObjId\": " + monsterObjId.ToString() + "," +
            "\"position\": " + position.jsonEncode() + "," +
            "\"appearCondId\": " + appearCondId.ToString() + ",";
			//"\"events\": [" + jEvents.TrimEnd(',') + "],";
		j = "{" + j.TrimEnd(',') + "}";
		return j;
	}
}

public class GenProp {
	public int propObjId;
	public Pos position;

	public string jsonEncode() {
		string j;
		j = "\"propObjId\": " + propObjId.ToString() + "," + 
			"\"position\": " + position.jsonEncode() + ",";
		j = "{" + j.TrimEnd(',') + "}";
		return j;
	}
}

public class GenTreasure {
	public int treasureId;
	public int treasureObjId;
	public Pos position;
	//public GenEvent[] events;

	public string jsonEncode() {
// 		string jEvents = "";
// 		if(events != null) {
// 			foreach(GenEvent e in events) {
// 				jEvents += e.jsonEncode() + ",";
// 			}
// 		}

		string j;
        j = "\"treasureId\": " + treasureId.ToString() + "," +
            "\"treasureObjId\": " + treasureObjId.ToString() + "," +
            "\"position\": " + position.jsonEncode() + ",";
			//"\"events\": [" + jEvents.TrimEnd(',') + "],";
		j = "{" + j.TrimEnd(',') + "}";
		return j;
	}
}

public class GenTrigger {
	public int triggerId;
	public int triggerObjId;
	public Pos position;
	public int triggerMethodId;
	//public GenEvent[] events;

	public string jsonEncode() {
// 		string jEvents = "";
// 		if(events != null) {
// 			foreach(GenEvent e in events) {
// 				jEvents += e.jsonEncode() + ",";
// 			}
// 		}

		string j;
        j = "\"triggerId\": " + triggerId.ToString() + "," +
            "\"triggerObjId\": " + triggerObjId.ToString() + "," +
            "\"position\": " + position.jsonEncode() + "," +
            "\"triggerMethodId\": " + triggerMethodId.ToString() + ",";
			//"\"events\": [" + jEvents.TrimEnd(',') + "],";
		j = "{" + j.TrimEnd(',') + "}";
		return j;
	}
}

public class GenTGate {
	public int teleportGateObjId;
	public Pos position;
	public string target;

	public string jsonEncode() {
		string j;
		j = "\"teleportGateObjId\": " + teleportGateObjId.ToString() + "," +
			"\"position\": " + position.jsonEncode() + "," +
			"\"target\": \"" + target + "\",";
		j = "{" + j.TrimEnd(',') + "}";
		return j;
	}
}

public class GenMonsterObj {
	public int monsterObjId;
	public int monsterSettingId;
	public GenSkill[] skills;

	public string jsonEncode() {
		string jSkill = "";
		if(skills != null) {
			foreach(GenSkill skill in skills) {
				jSkill += skill.jsonEncode() + ",";
			}
		}

		string j;
		j = "\"monsterObjId\": " + monsterObjId.ToString() + "," +
			"\"monsterSettingId\": " + monsterSettingId.ToString() + "," +
			"\"skills\": [" + jSkill.TrimEnd(',') + "]" + ",";
		j = "{" + j.TrimEnd(',') + "}";
		return j;
	}
}

public class GenSkill {
	public int skillId;
	public int[] skillParam;

	public string jsonEncode() {
		string jSkillParam = "";
		if(skillParam != null) {
			foreach(int param in skillParam) {
				jSkillParam += param.ToString() + ",";
			}
		}

		string j;
		j = "\"skillId\": " + skillId.ToString() + "," +
			"\"skillParam\": [" + jSkillParam.TrimEnd(',') + "]" + ",";
		j = "{" + j.TrimEnd(',') + "}";
		return j;
	}
}

public class GenVariable {
	public int variableId;
	public int initValue;
	//public GenEvent triggerEvent;

	public string jsonEncode() {
		string j;
        j = "\"variableId\": " + variableId.ToString() + "," +
            "\"initValue\": " + initValue.ToString() + ",";
			//(triggerEvent!=null?"\"event\": " + triggerEvent.jsonEncode() + ",":"");
		j = "{" + j.TrimEnd(',') + "}";
		return j;
	}
}
public class GenEvents
{
    public enum TriggerType
    {
        MONSTER_DIE = 1,
        MONSTER_HP_BELOW = 2,
        MONSTER_USE_SKILL = 3,

        TREASURE_OPENED = 101,

        TRIGGER_OPENED = 201,

        GATE_PLAYER_NEAR = 301,

        LEVEL_ENTER = 901
    };
    public int eventId;
    public TriggerType triggerTypeId;
    public int levelId;
    public int roomId;
    public int[] triggerParams;
    public string jsonEncode()
    {
        string jTriggerParams = "";
		if(triggerParams != null) {
			foreach(int param in triggerParams) {
				jTriggerParams += param.ToString() + ",";
			}
		}
        string j;
        j = "\"eventId\": " + eventId.ToString() + "," +
            "\"triggerTypeId\": " + ((int)triggerTypeId).ToString() + "," +
            "\"levelId\": " + levelId.ToString() + "," +
            "\"roomId\": " + roomId.ToString() + "," +
            "\"params\": [" + jTriggerParams.TrimEnd(',') + "]" + ","
            ;
        j = "{" + j.TrimEnd(',') + "}";
        return j;
    }
}

public class GenEResults
{
    public enum ResultType
    {
        MONSTER_SPAWN = 1,

        TRIGGER_SKILL = 101,

        GATE_OPEN = 201,

        FLOOR_CLEAR = 301,

        DROP_MONEY = 801,
        DROP_MATERIAL = 802,
        DROP_CHARACTER = 803,
        DROP_HEALTHPACK = 804,
        DROP_KEY = 805,

        VAR_SET_AS_NUM = 901,
        VAR_SET_AS_VAR_PLUS_NUM = 902,
        VAR_SET_AS_VAR_PLUS_VAR_PLUS_NUM = 903,
        VAR_SET_AS_VAR_MINUS_VAR_PLUS_NUM = 904
    };
    public ResultType resultTypeId;
    public int eventId;
    public int[] resultParams;
    public string jsonEncode()
    {
        string jTriggerParams = "";
        if (resultParams != null)
        {
            foreach (int param in resultParams)
            {
                jTriggerParams += param.ToString() + ",";
            }
        }
        string j;
        j = "\"resultTypeId\": " + ((int)resultTypeId).ToString() + "," +
            "\"eventId\": " + eventId.ToString() + "," +
            "\"params\": [" + jTriggerParams.TrimEnd(',') + "]" + ","
            ;
        j = "{" + j.TrimEnd(',') + "}";
        return j;
    }
}


// public class GenEvent {
// 	public enum TriggerType {
// 		MONSTER_DIE 		= 1,
// 		MONSTER_HP_BELOW 	= 2,
// 		MONSTER_USE_SKILL 	= 3,
// 		
// 		TREASURE_OPENED 	= 101,
// 		
// 		TRIGGER_OPENED 		= 201,
// 		
// 		GATE_PLAYER_NEAR 	= 301,
// 		
// 		LEVEL_ENTER 		= 901
// 	};
// 	public enum CondType {
// 		LOGIC_LARGER_OR_EQUAL_NUM = 1,
// 		LOGIC_LARGER_OR_EQUAL_VAR = 2,
// 		
// 		PLAYER_HAS_KEY 		= 101
// 	};
// 	public enum ResultType {
// 		MONSTER_SPAWN 		= 1,
// 		
// 		TRIGGER_SKILL 		= 101,
// 		
// 		GATE_OPEN 			= 201,
// 
// 		FLOOR_CLEAR			= 301,
// 		
// 		DROP_MONEY 			= 801,
// 		DROP_MATERIAL 		= 802,
// 		DROP_CHARACTER 		= 803,
// 		DROP_HEALTHPACK 	= 804,
// 		DROP_KEY 			= 805,
// 		
// 		VAR_SET_AS_NUM 						= 901,
// 		VAR_SET_AS_VAR_PLUS_NUM 			= 902,
// 		VAR_SET_AS_VAR_PLUS_VAR_PLUS_NUM 	= 903,
// 		VAR_SET_AS_VAR_MINUS_VAR_PLUS_NUM 	= 904
// 	};
// 	public enum CondsRelationType {
// 		AND = 0,
// 		OR = 1
// 	}
// 
// 	public int eventId;
// 	public TriggerType triggerTypeId;
// 	public float[] triggerParams;
// 	public GenEventCond[] conds;
// 	public int condsRelation;
// 	public GenEventResult[] results;
// 
// 	public string jsonEncode() {
// 		string jTriggerParams = "";
// 		if(triggerParams != null) {
// 			foreach(int param in triggerParams) {
// 				jTriggerParams += param.ToString() + ",";
// 			}
// 		}
// 		string jConds = "";
// 		if(conds != null) {
// 			foreach(GenEventCond cond in conds) {
// 				jConds += cond.jsonEncode() + ",";
// 			}
// 		}
// 		string jResults = "";
// 		if(results != null) {
// 			foreach(GenEventResult result in results) {
// 				jResults += result.jsonEncode() + ",";
// 			}
// 		}
// 		
// 		string j;
// 		j = "\"eventId\": " + eventId.ToString() + "," +
// 			"\"triggerTypeId\": " + ((int)triggerTypeId).ToString() + "," +
// 			"\"params\": [" + jTriggerParams.TrimEnd(',') + "]" + "," +
// 			"\"conds\": [" + jConds.TrimEnd(',') + "]" + "," +
// 			"\"condsRelation\": " + condsRelation.ToString() + "," +
// 			"\"results\": [" + jResults.TrimEnd(',') + "]" + ",";
// 		j = "{" + j.TrimEnd(',') + "}";
// 		return j;
// 	}
// }
// 
// public class GenEventCond {
// 	public GenEvent.CondType condTypeId;
// 	public float[] condParams;
// 
// 	public string jsonEncode() {
// 		string jCondParams = "";
// 		if(condParams != null) {
// 			foreach(int param in condParams) {
// 				jCondParams += param.ToString() + ",";
// 			}
// 		}
// 		
// 		string j;
// 		j = "\"condTypeId\": " + ((int)condTypeId).ToString() + "," +
// 			"\"params\": [" + jCondParams.TrimEnd(',') + "]" + ",";
// 		j = "{" + j.TrimEnd(',') + "}";
// 		return j;
// 	}
// }
// 
// public class GenEventResult {
// 	public GenEvent.ResultType resultTypeId;
// 	public float[] resultParams;
// 	
// 	public string jsonEncode() {
// 		string jCondParams = "";
// 		if(resultParams != null) {
// 			foreach(int param in resultParams) {
// 				jCondParams += param.ToString() + ",";
// 			}
// 		}
// 		
// 		string j;
// 		j = "\"resultTypeId\": " + ((int)resultTypeId).ToString() + "," +
// 			"\"params\": [" + jCondParams.TrimEnd(',') + "]" + ",";
// 		j = "{" + j.TrimEnd(',') + "}";
// 		return j;
// 	}
// }
