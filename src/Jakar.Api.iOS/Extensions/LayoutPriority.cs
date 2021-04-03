#pragma warning disable 1591

#nullable enable
namespace Jakar.Api.iOS.Extensions
{
	public enum LayoutPriority // iOS only
	{
		Zero = 0,           // 0
		Minimum = 1,        // 1
		Lowest = 50,        // (float) UILayoutPriority.FittingSizeLevel,                   
		Low = 250,          // (float) UILayoutPriority.DefaultLow,                              
		BelowAverage = 490, // (float) UILayoutPriority.DragThatCannotResizeScene; 
		Average = 500,      // (float) UILayoutPriority.SceneSizeStayPut;        
		AboveAverage = 510, // (float) UILayoutPriority.DragThatCanResizeScene;
		High = 750,         // (float) UILayoutPriority.DefaultHigh;                           
		Highest = 999,      // 999
		Required = 1000,    //  (float) UILayoutPriority.Required; 
	}
}
