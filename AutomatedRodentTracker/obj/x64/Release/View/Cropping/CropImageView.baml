   M S B A M L   `   `   `  ���� Q  MAutomatedRodentTracker, Version=1.0.0.0, Culture=neutral, PublicKeyToken=nullU/clr-namespace:AutomatedRodentTracker.Behaviours!AutomatedRodentTracker.Behaviours  8    2AutomatedRodentTracker.View.Cropping.CropImageView   W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     @d2http://schemas.microsoft.com/expression/blend/2008     Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     F
behaviours/clr-namespace:AutomatedRodentTracker.Behaviours       � Title$  CropImageView��$	��720��$
��1280�� � WindowState$ 	Maximized=�=   7AutomatedRodentTracker.Behaviours.WindowClosingBehavior   Close ��  �� Path$ Close�   Closing �� $ ClosingCommand�.��� -    �Name$
 Grid�� � RowDefinitions �� $��* ��� $
��Auto �.+��� -    ��Name$ Image��$��0��$	��Top=�$��Center=����� $ DisplayImage� �� NotifyOnTargetUpdated . �� -   	 ��Name$	 Canvas��$��0��.+��� -   
 ��Name$
 	Rectangle��
���  �� �� StrokeDashArray$ 1R�$��1���� -   $
 PixelRectangle��$y�Hidden=� �� Left �� $ X� �� Mode$ TwoWay=�
 �� Top �� $ Y�$ TwoWay=����� $ Width�$ TwoWay=����� $ Height�$ TwoWay=���  �� Orientation$ 
Horizontal=�$��1��$��Center=�.+��� $��Ok������ $ 	OkCommand��� $��Cancel������ $ CancelCommand�