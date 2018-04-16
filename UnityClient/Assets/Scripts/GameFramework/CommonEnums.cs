using UnityEngine;
using System.Collections;

public enum ENSerializeMask : int
{
	enSerializeSaveable = 0x1 << 1,
	enSerializeDirty = 0x1 << 2,
	enSerializeSave = 0x1 << 5,
	enSerializeClearDirty = 0x1 << 16,
};

//序列化哪些内容
enum ENSerilizeType : int
{
	enSerilizeProps = 0x1 << 1,     //序列化属性
	enSerilizeItemBag = 0x1 << 2,   //序列化背包
	enSerilizeFloorInsts = 0x1 << 3,//序列化关卡进度

	enSerilizeAll = enSerilizeProps | enSerilizeItemBag | enSerilizeFloorInsts,
};
