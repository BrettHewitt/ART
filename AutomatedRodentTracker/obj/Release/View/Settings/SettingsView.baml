   M S B A M L   `   `   `  ���� Q  MAutomatedRodentTracker, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null[2clr-namespace:AutomatedRodentTracker.View.Settings$AutomatedRodentTracker.View.Settings  U/clr-namespace:AutomatedRodentTracker.Behaviours!AutomatedRodentTracker.Behaviours  U/clr-namespace:AutomatedRodentTracker.Converters!AutomatedRodentTracker.Converters  7    1AutomatedRodentTracker.View.Settings.SettingsView   R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     @d2http://schemas.microsoft.com/expression/blend/2008     Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     Dlocal2clr-namespace:AutomatedRodentTracker.View.Settings     F
behaviours/clr-namespace:AutomatedRodentTracker.Behaviours     F
converters/clr-namespace:AutomatedRodentTracker.Converters       � Title$  SettingsView��$	��480��$	��640��=   7AutomatedRodentTracker.Behaviours.WindowClosingBehavior   Close ��  �� Path$ Close� �� Mode$ TwoWay=�   Closing �� $ ClosingCommand� � 	Resources D   >AutomatedRodentTracker.Converters.BooleanToVisibilityConverter    BooleanToVisibilityConverterK   EAutomatedRodentTracker.Converters.InverseBooleanToVisibilityConverter ' #InverseBooleanToVisibilityConverter �� 
TargetType �� Property �� Value��%�   &	        &	      '�� 
     '�� ,     '�� h         �� " ��.���  � ��$ 2q��� " ��.���  � ��$ Center=���  � ��$ 1q��� " ��.���  � ��$ 1q�.��� 	 � RowDefinitions	 �� $��* ��� $
��Auto �.+�� $��0��	 �� $��* ��� $
��Auto �
 � ColumnDefinitions
 �� $��* ��� $��* �.+��� $��0��$��2��$��0��$��2q����� $
 Mice� �� SelectedItem �� $ SelectedMouse�$ TwoWay=� �� View �� .P��� $
��File�� �� Width$	 120�� �� DisplayMemberBinding �� *VideoFileName+�� 
�������$��1q�$��2q� �� Padding$ 2q�$��0��$��1��._�� $��2q�y��� $ 	ShowVideo� �� 	Converter# [  	 �� $��* ��� $
��Auto �.+��� $��0������ $ DisplayImage��� $��1�� � Value �� $ SliderValue�$ TwoWay=� � Minimum �� $ Minimum� � Maximum �� $ Maximum��� 
�������$��1q�$��2q�$ 2q�$��1��$��1��{��� $ 	ShowVideo�._�� 	 �� $
��Auto ��� $
��Auto ��� $
��Auto ��� $
��Auto ��� $
��Auto ��� $
��Auto ��� $
��Auto �
 �� $
��Auto ��� $��* �.+��� $��0��$��0��$��Gap Distance: ���� $��0��$��1�� �� $ GapDistance�$ TwoWay=� �� $ GapDistanceMin� �� $ GapDistanceMax��� $��1��$��0��$��Binary Treshold: ���� $��1��$��1�� �� $ BinaryThreshold�$ TwoWay=� �� $ BinaryThresholdMin� �� $ BinaryThresholdMax��� $��2��$��0��$��Binary Treshold 2: ���� $��2��$��1�� �� $ BinaryThreshold2�$ TwoWay=� �� $ BinaryThreshold2Min� �� $ BinaryThreshold2Max��� $��Smooth Motion: ��$��3��$��0���� $��3��$��1��$��Center=�$��2q� d� 	IsChecked �� $ SmoothMotion�$ TwoWay=��� $��Frame Rate: ��$��4��$��0���� $��4��$��1������ $ 	FrameRate�$ TwoWay=��� $��1�� �� Orientation$ 
Horizontal=�$��Center=�.+��� $��Ok������ $ 	OkCommand��� $��Cancel������ $ CancelCommand�