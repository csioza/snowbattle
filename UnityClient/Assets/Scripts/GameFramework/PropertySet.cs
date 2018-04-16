using NGE.Network;
using System;
using System.Collections.Generic;
using System.IO;

//!属性对应的自定义对象
public abstract class PropertyCustomValue
{
	public abstract void Read(BinaryHelper stream, int fixSize);
	public abstract byte[] Write();

	public abstract void ReadFromString(string value);
	public abstract string WriteToString();
};
//!自定义属性的工厂方法
public abstract class PropertyValueFactory
{
	public abstract PropertyCustomValue CreateValue();
};


//!属性类型
public enum PropertySetValueType
	: int
{
	enInt32,
	enFloat,
	enString,
	enFloat3,
	enBitArray,
	enBuffer,
	enInt64,
};
public enum ENPropertyNotifyPipe
{
	enNotifyPipeNone = 0,
	enNotifyPipeSelf = 0x1 << 1,
	enNotifyPipeAround = 0x1 << 2,
	enNotifyPipeScene = 0x1 << 3,
	enNotifyPipeWorld = 0x1 << 4,
	enNotifyPipeSerializeSave = 0x1 << 5,
	enNotifyPipeWatch = 0x1 << 6,
};

public struct Float3
{
	public float x;
	public float y;
	public float z;
};
public class PropertyValue : ICloneable
{
	//Value
	public int m_int32;
	public float m_float;
	public Float3 m_float3;
	public string m_string;
	public PropertyCustomValue m_customObject;
	public int m_customObjectFixLength;

	public bool m_isDirty;
	public int m_propertyID;
	public int m_type;

	bool IsDynamicSize()
	{
		return m_type == (int)PropertySetValueType.enString;
	}
	public object Clone()
	{
		return this.MemberwiseClone();
	}
};
public class PropertyDef
{
	public string m_name;
	public int m_propertyID;
	public int m_notifyPipeMask;
	//!使用了多少buffer字节
	public int m_usedBufferBytes;
	public PropertyValueFactory m_valueFactory;
	//!类型
	public int m_type;
};

public class PropertySetDefine
{
    //key为propertyID
    Dictionary<int, PropertyDef> m_defines = new Dictionary<int, PropertyDef>();
	PropertySetClass m_allPropertyClass;
	public PropertySetDefine()
	{
		m_allPropertyClass = new PropertySetClass(this);
	}
	int OnPropertyAdded(PropertyDef def, PropertyValue v, int propertyID = 0)
	{
		def.m_propertyID = propertyID == 0 ? m_defines.Count : propertyID;
		m_defines[def.m_propertyID] = def;
		v.m_type = def.m_type;
		PropertySet propDefault = m_allPropertyClass.GetDefaultProperty();
		return propDefault.AddPropertyIn(v, def.m_propertyID);
	}
	public PropertyDef GetPropertyDefine(int propertyID)
	{
        if (m_defines.ContainsKey(propertyID))
		{
            return m_defines[propertyID];
        }
		return new PropertyDef();
	}
	public int AddProperty_Int32(string name, int value, int propertyID = 0)
	{
		PropertyDef def = new PropertyDef();
		def.m_name = name;
		def.m_type = (int)PropertySetValueType.enInt32;
		PropertyValue v = new PropertyValue();
		v.m_int32 = value;
		OnPropertyAdded(def, v, propertyID);
		return def.m_propertyID;
	}
    public int AddProperty_Float(string name, float defaultValue, int propertyID = 0)
	{
		PropertyDef def = new PropertyDef();
		def.m_name = name;
		def.m_type = (int)PropertySetValueType.enFloat;
		PropertyValue v = new PropertyValue();
		v.m_float = defaultValue;
		OnPropertyAdded(def, v, propertyID);
		return def.m_propertyID;
	}
    public int AddProperty_Float3(string name, Float3 defaultValue, int propertyID = 0)
	{
		PropertyDef def = new PropertyDef();
		def.m_name = name;
		def.m_type = (int)PropertySetValueType.enFloat3;
		PropertyValue v = new PropertyValue();
		v.m_float3 = defaultValue;
		OnPropertyAdded(def, v, propertyID);
		return def.m_propertyID;
	}
    public int AddProperty_String(string name, string defaultValue, int fixLength, int propertyID = 0)
	{
		PropertyDef def = new PropertyDef();
		def.m_name = name;
		def.m_type = (int)PropertySetValueType.enString;
		PropertyValue v = new PropertyValue();
		v.m_string = defaultValue;
		v.m_customObjectFixLength = fixLength;
		OnPropertyAdded(def, v, propertyID);
		return def.m_propertyID;
	}
    public int AddProperty_BitArray(string name, PropertyValueFactory factory, int bitCount, int propertyID = 0)
	{
		PropertyDef def = new PropertyDef();
		def.m_name = name;
		def.m_type = (int)PropertySetValueType.enBitArray;
		def.m_usedBufferBytes = bitCount / 8;
		def.m_valueFactory = factory;
		PropertyValue v = new PropertyValue();
		v.m_customObject = factory.CreateValue();
		v.m_customObjectFixLength = bitCount / 8;
		OnPropertyAdded(def, v, propertyID);
		return def.m_propertyID;
	}
    public int AddProperty_Buffer(string name, PropertyValueFactory factory, int bytesCount, int propertyID = 0)
	{
		PropertyDef def = new PropertyDef();
		def.m_name = name;
		def.m_type = (int)PropertySetValueType.enBuffer;
		def.m_usedBufferBytes = bytesCount;
		def.m_valueFactory = factory;
		PropertyValue v = new PropertyValue();
		v.m_customObject = factory.CreateValue();
		v.m_customObjectFixLength = bytesCount;
		OnPropertyAdded(def, v, propertyID);
		return def.m_propertyID;
	}

    public int AddProperty_Int64(string name, int defaultVLow, int defaultVHigh, int propertyID = 0)
	{
		PropertyDef def = new PropertyDef();
		def.m_name = name;
		def.m_type = (int)PropertySetValueType.enInt64;
		def.m_usedBufferBytes = 8;
		def.m_valueFactory = PropertyValueInt64Factory.Singleton;
		PropertyValue v = new PropertyValue();
		PropertyValueInt64 value = PropertyValueInt64Factory.Singleton.CreateValue() as PropertyValueInt64;
		value.SetInt64(defaultVLow, defaultVHigh);
		v.m_customObject =value;
		v.m_customObjectFixLength = 8;
		OnPropertyAdded(def, v, propertyID);
		return def.m_propertyID;
	}
}

public class PropertySetClass
{
    public List<int> m_propertyID2Index = new List<int>();
	public PropertySet m_default;
	//!需要可视化的属性
	public List<int> m_viewableProperty = new List<int>();
	//!需要保存的属性
	public List<int> m_saveProperty = new List<int>();
	//!
	public PropertySetDefine m_def;
	//public List<PropertySet> m_cachePropertySet = new List<PropertySet>();
	public PropertySetClass(PropertySetDefine def)
	{
		m_def = def;
		m_default = new PropertySet(this);
	}
	public PropertySet GetDefaultProperty()
	{
		return m_default;
	}
	public void AddProperty(int propertyID, int notifyPipe)
	{
		PropertyValue v = new PropertyValue();
		int index = m_default.AddPropertyIn(v, propertyID);
		if ((notifyPipe & (int)ENPropertyNotifyPipe.enNotifyPipeSerializeSave) > 0)
		{
			m_saveProperty.Add(index);
		}
		if ((notifyPipe & (int)ENPropertyNotifyPipe.enNotifyPipeAround) > 0)
		{
			m_viewableProperty.Add(index);
		}
	}
	public PropertySet CreateInstance()
	{
		PropertySet p = new PropertySet(this, m_default);

		return p;
	}
	public void ReleaseInstance(PropertySet propertySet)
	{

	}
}

public class PropertySet
{
	public static int flagNum = 1;
	public int myFlagNum;

	PropertySetClass m_class;
	List<PropertyValue> m_propertys = new List<PropertyValue>();
	//!变动的属性列表
	List<int> m_dirtyPropertys = new List<int>();
	bool m_isRecordDirty;
	public bool IsRecordDirty
	{
		get { return m_isRecordDirty; }
		set { m_isRecordDirty = value; }
	}
	public List<int> DirtyPropertys
	{
		get { return m_dirtyPropertys; }
	}
	public PropertySet(PropertySetClass theClass)
	{
		m_class = theClass;
	}
	public PropertySet(PropertySetClass theClass, PropertySet copyFrom)
	{
		flagNum++;
		myFlagNum = flagNum;
		m_class = theClass;
		foreach (PropertyValue value in copyFrom.m_propertys)
		{
			PropertyValue v = new PropertyValue();
			v = (PropertyValue)value.Clone();
			v.m_customObject = null;
			m_propertys.Add(v);
		}
	}
	public int AddPropertyIn(PropertyValue value_, int propertyID)
    {
		int index = m_propertys.Count;
		value_.m_propertyID = propertyID;
		PropertyDef def = m_class.m_def.GetPropertyDefine(propertyID);
		value_.m_type = def.m_type;
        int count = m_class.m_propertyID2Index.Count;
        for (int i=count;i<propertyID+1;i++)
        {
            m_class.m_propertyID2Index.Add(-1);
        }
		m_class.m_propertyID2Index[propertyID] = index;
		m_propertys.Add(value_);
		return index;
	}

	public PropertyValue GetPropertyByID(int propertyID, ref int index)
	{
        if (propertyID < m_class.m_propertyID2Index.Count)
		{
			index = m_class.m_propertyID2Index[propertyID];
            if (index < 0)
            {
                return null;
            }
			PropertyValue value = m_propertys[index];
			if (value.m_type == (int)PropertySetValueType.enBuffer || value.m_type == (int)PropertySetValueType.enBitArray || value.m_type == (int)PropertySetValueType.enInt64)
			{
				if (value.m_customObject == null)
				{
					PropertyDef def = m_class.m_def.GetPropertyDefine(propertyID);
					if (def.m_valueFactory != null)
					{
						value.m_customObject = def.m_valueFactory.CreateValue();
					}
				}
			}
			return value;
		}
		return null;
	}
	public void Serialize(PacketWriter stream)
	{
		stream.Write(0);
		stream.WriteCompressInt(m_propertys.Count);
		foreach (var item in m_propertys)
		{
			stream.WriteCompressInt(item.m_propertyID);
			switch (item.m_type)
			{
				case (int)PropertySetValueType.enBitArray:
				case (int)PropertySetValueType.enBuffer:
					byte[] customBytes = item.m_customObject.Write();
					stream.Write(customBytes.Length);
					stream.Write(item.m_customObject.Write());
					break;
				case (int)PropertySetValueType.enString:
					stream.WriteCompressInt(item.m_string.Length * sizeof(char));
					stream.WriteUTF8(item.m_string);
					break;
				case (int)PropertySetValueType.enInt32:
					stream.WriteCompressInt(sizeof(int));
					stream.WriteCompressInt(item.m_int32);
					break;
				case (int)PropertySetValueType.enFloat:
					stream.WriteCompressInt(sizeof(float));
					stream.Write(item.m_float);
					break;
				case (int)PropertySetValueType.enFloat3:
					stream.WriteCompressInt(sizeof(float) * 3);
					stream.Write(item.m_float3.x);
					stream.Write(item.m_float3.y);
					stream.Write(item.m_float3.z);
					break;
				case (int)PropertySetValueType.enInt64:
					byte[] customBytes64 = item.m_customObject.Write();
					stream.Write(customBytes64.Length);
					stream.Write(item.m_customObject.Write());
					break;
			}
		}
	}
	public void Deserialize(PacketReader stream)
	{
		int operatorMask = 0;
		operatorMask = stream.ReadInt32();
		int count = stream.ReadCompressInt();
		if (((int)ENSerializeMask.enSerializeDirty & operatorMask) > 0)
		{
			m_dirtyPropertys.Clear();
		}
		for (int i = 0; i < count; i++)
		{
			int propertyID = 0;
			int size = 0;
			propertyID = stream.ReadCompressInt();
			size = stream.ReadCompressInt();
			int index = 0;
			PropertyValue prop = GetPropertyByID(propertyID, ref index);
			if (prop == null)
			{
				stream.Seek(size, SeekOrigin.Current);
				continue;
			}
			switch (prop.m_type)
			{
				case (int)PropertySetValueType.enBitArray:
				case (int)PropertySetValueType.enBuffer:
					//stream.ReadBuffer(2);
					//prop.m_customObject.Read(stream, size);
					//stream.ReadBuffer(2);
					byte[] bytes = stream.ReadBuffer(size);
					BinaryHelper innerStream = new BinaryHelper(bytes);
					prop.m_customObject.Read(innerStream, size);
					break;
				case (int)PropertySetValueType.enString:
					prop.m_string = stream.ReadUTF8(size);
					break;
				case (int)PropertySetValueType.enInt32:
					prop.m_int32 = stream.ReadInt32();
					break;
				case (int)PropertySetValueType.enFloat:
					prop.m_float = stream.ReadFloat();
					break;
				case (int)PropertySetValueType.enFloat3:
					prop.m_float3.x = stream.ReadFloat();
					prop.m_float3.y = stream.ReadFloat();
					prop.m_float3.z = stream.ReadFloat();
					break;
				case (int)PropertySetValueType.enInt64:
					byte[] bytes64 = stream.ReadBuffer(size);
					BinaryHelper innerStream64 = new BinaryHelper(bytes64);
					prop.m_customObject.Read(innerStream64, size);
					break;
			}

			if (((int)ENSerializeMask.enSerializeDirty & operatorMask) > 0)
			{
				m_dirtyPropertys.Add(propertyID);
			}
		}
	}
	public string SerializeToText()
	{
		StringStreamWriter writer = new StringStreamWriter();
		writer.Write(m_propertys.Count);
		foreach (var item in m_propertys)
		{
			writer.Write(item.m_propertyID);
			switch (item.m_type)
			{
				case (int)PropertySetValueType.enBitArray:
				case (int)PropertySetValueType.enBuffer:
					writer.Write(item.m_customObject.WriteToString());
					break;
				case (int)PropertySetValueType.enString:
					writer.Write(item.m_string);
					break;
				case (int)PropertySetValueType.enInt32:
					writer.Write(item.m_int32);
					break;
				case (int)PropertySetValueType.enFloat:
					writer.Write(item.m_float);
					break;
				case (int)PropertySetValueType.enFloat3:
					writer.Write(item.m_float3.x);
					writer.Write(item.m_float3.y);
					writer.Write(item.m_float3.z);
					break;
			}
		}
		return writer.ToString();
	}
	public void DeserializeFromText(string text)
	{
		StringStreamReader reader = new StringStreamReader(text);
		int count = reader.ReadInt();
		for (int index = 0; index < count; ++index)
		{
			int propertyID = reader.ReadInt();
			int innerIndex = 0;
			PropertyValue prop = GetPropertyByID(propertyID, ref innerIndex);
			if (prop == null)
			{
				continue;
			}
			switch (prop.m_type)
			{
				case (int)PropertySetValueType.enBitArray:
				case (int)PropertySetValueType.enBuffer:
					prop.m_customObject.ReadFromString(reader.ReadString());
					break;
				case (int)PropertySetValueType.enString:
					prop.m_string = reader.ReadString();
					break;
				case (int)PropertySetValueType.enInt32:
					prop.m_int32 = reader.ReadInt();
					break;
				case (int)PropertySetValueType.enFloat:
					prop.m_float = reader.ReadFloat();
					break;
				case (int)PropertySetValueType.enFloat3:
					prop.m_float3.x = reader.ReadFloat();
					prop.m_float3.y = reader.ReadFloat();
					prop.m_float3.z = reader.ReadFloat();
					break;
			}
		}
	}
	void OnPropertyChanged(PropertyValue v, int propertyID)
	{
		if (m_isRecordDirty)
		{
			m_dirtyPropertys.Add(propertyID);
		}
	}

	public int GetProperty_Int32(int propertyID)
	{
		int index = 0;
		PropertyValue prop = GetPropertyByID(propertyID, ref index);
		if (prop != null && prop.m_type == (int)PropertySetValueType.enInt32)
		{
			return prop.m_int32;
		}
		return 0;
	}
	public bool SetProperty_Int32(int propertyID, int v)
	{
		int index = 0;
		PropertyValue prop = GetPropertyByID(propertyID, ref index);
		if (prop != null && prop.m_type == (int)PropertySetValueType.enInt32)
		{
			if (prop.m_int32 != v)
			{
				OnPropertyChanged(prop, index);
			}
			prop.m_int32 = v;
			return true;
		}
		return false;
	}
	public float GetProperty_Float(int propertyID)
	{
		int index = 0;
		PropertyValue prop = GetPropertyByID(propertyID, ref index);
		if (prop != null && prop.m_type == (int)PropertySetValueType.enFloat)
		{
			return prop.m_float;
		}
		return 0.0f;
	}
	public bool SetProperty_Float(int propertyID, float v)
	{
		int index = 0;
		PropertyValue prop = GetPropertyByID(propertyID, ref index);
		if (prop != null && prop.m_type == (int)PropertySetValueType.enFloat)
		{
			OnPropertyChanged(prop, index);
			prop.m_float = v;
			return true;
		}
		return false;
	}
	public Float3 GetProperty_Float3(int propertyID)
	{
		int index = 0;
		PropertyValue prop = GetPropertyByID(propertyID, ref index);
		if (prop != null && prop.m_type == (int)PropertySetValueType.enFloat3)
		{
			return prop.m_float3;
		}
		Float3 f3 = new Float3();
		return f3;
	}
	public bool SetProperty_Float3(int propertyID, Float3 v)
	{
		int index = 0;
		PropertyValue prop = GetPropertyByID(propertyID, ref index);
		if (prop != null && prop.m_type == (int)PropertySetValueType.enFloat3)
		{
			OnPropertyChanged(prop, index);
			prop.m_float3 = v;
			return true;
		}
		return false;
	}
	public string GetProperty_String(int propertyID)
	{
		int index = 0;
		PropertyValue prop = GetPropertyByID(propertyID, ref index);
		if (prop != null && prop.m_type == (int)PropertySetValueType.enString)
		{
			return prop.m_string;
		}
		return "";
	}
	public bool SetProperty_String(int propertyID, string v)
	{
		int index = 0;
		PropertyValue prop = GetPropertyByID(propertyID, ref index);
		if (prop != null && prop.m_type == (int)PropertySetValueType.enString)
		{
			prop.m_string = v;
		}
		return false;
	}
	public PropertyCustomValue GetProperty_Custom(int propertyID)
	{
		int index = 0;
		PropertyValue prop = GetPropertyByID(propertyID, ref index);
		if (prop != null && (prop.m_type == (int)PropertySetValueType.enBitArray
			|| prop.m_type == (int)PropertySetValueType.enBuffer)
			)
		{
			return prop.m_customObject;
		}
		return null;
	}
	public void SetProperty_Custom(int propertyID, PropertyCustomValue v)
	{
		int index = 0;
		PropertyValue prop = GetPropertyByID(propertyID, ref index);
		if (prop != null && (prop.m_type == (int)PropertySetValueType.enBitArray
			|| prop.m_type == (int)PropertySetValueType.enBuffer)
			)
		{
			prop.m_customObject = v;
			OnPropertyChanged(prop, index);
		}
	}


	public MyInt64 GetProperty_Int64(int propertyID)
	{
		int index = 0;
		PropertyValue prop = GetPropertyByID(propertyID, ref index);
		if (prop != null && prop.m_type == (int)PropertySetValueType.enInt64)
		{
			PropertyValueInt64 val64 = prop.m_customObject as PropertyValueInt64;
			return val64.m_value;
		}
		return new MyInt64();
	}

	public long SetProperty_Int64(int propertyID, int vLow, int vHigh)
	{
		int index = 0;
		PropertyValue prop = GetPropertyByID(propertyID, ref index);
		if (prop != null && prop.m_type == (int)PropertySetValueType.enInt64)
		{
			PropertyValueInt64 val64 = prop.m_customObject as PropertyValueInt64;
			val64.SetInt64(vLow, vHigh);
			OnPropertyChanged(prop, index);
		}
		return 0;
	}

}
