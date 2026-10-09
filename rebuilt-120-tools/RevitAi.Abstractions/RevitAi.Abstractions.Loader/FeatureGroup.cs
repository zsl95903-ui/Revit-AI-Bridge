namespace RevitAi.Abstractions.Loader;

public enum FeatureGroup
{
	[GroupInfo("核心功能", "#2196F3", "core")]
	CoreFeatures,
	[GroupInfo("AI智能设计", "#7C4DFF", "ai")]
	AIDesign,
	[GroupInfo("房间与建筑方案", "#F44336", "room")]
	RoomAndBuilding,
	[GroupInfo("标高轴网与基准", "#FF9800", "grid")]
	LevelAndGrid,
	[GroupInfo("墙梁柱板结构", "#8D6E63", "structure")]
	WallBeamColumn,
	[GroupInfo("机电管线综合", "#2196F3", "mep")]
	MEPPiping,
	[GroupInfo("尺寸标注与注释", "#00BCD4", "dimension")]
	DimensionAndAnnotation,
	[GroupInfo("视图图纸与出图", "#4CAF50", "view")]
	ViewAndSheet,
	[GroupInfo("族类型参数管理", "#E91E63", "family")]
	FamilyAndParameter,
	[GroupInfo("批量拆分与合并", "#3F51B5", "batch")]
	BatchSplitAndMerge,
	[GroupInfo("表格数据互通", "#FFC107", "data")]
	DataExchange,
	[GroupInfo("工具辅助系统", "#607D8B", "tool")]
	ToolAssistant,
	[GroupInfo("路桥市政与基建", "#0277BD", "infrastructure")]
	Infrastructure
}
