   M S B A M L   `   `   `  ���� Q  MAutomatedRodentTracker, Version=1.0.0.0, Culture=neutral, PublicKeyToken=nullU/clr-namespace:AutomatedRodentTracker.Behaviours!AutomatedRodentTracker.Behaviours  U/clr-namespace:AutomatedRodentTracker.Converters!AutomatedRodentTracker.Converters  @    :AutomatedRodentTracker.View.BatchProcess.Review.ReviewView   W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     F
behaviours/clr-namespace:AutomatedRodentTracker.Behaviours     F
converters/clr-namespace:AutomatedRodentTracker.Converters       � Title$  
ReviewView��$	��480��$	��640�� � WindowState$ 	Maximized=�=   7AutomatedRodentTracker.Behaviours.WindowClosingBehavior   Close ��  �� Path$ Close� �� Mode$ TwoWay=�   Closing �� $ ClosingCommand� � 	Resources D   >AutomatedRodentTracker.Converters.BooleanToVisibilityConverter    BooleanToVisibilityConverterK   EAutomatedRodentTracker.Converters.InverseBooleanToVisibilityConverter ' #InverseBooleanToVisibilityConverter �� 
TargetType �� Property	 �� Value��%�   &	        &	      '�� 
     '�� ,     '�� h         �� " ��.���  � ��$	 2q��� " ��.���  � ��$	 Center=���  � ��$	 1q��� " ��.���  � ��$	 1q�.��� 
 � RowDefinitions
 �� $��* ��� $
��Auto �.+�� $��0��
 �� $��* ��� $��* � � ColumnDefinitions �� $��* ��� $��* �.+��� $��0��$��0��$��2q����� $ SingleFiles� �� SelectedItem �� $ SelectedVideo�$ TwoWay=� �� View �� .P��� $
��File�� �� Width$	 120�� �� DisplayMemberBinding �� *VideoFileName+�� $��Result��$ 50�� �� *VideoOutcome+�� $��Comments��$	 150�� �� *
Comments+�� 
�������$��1q�$��2q� �� Padding$ 2q�$��0��$��1��._�� $��2q�y��� $ 	ShowVideo� �� 	Converter# [  
 �� $��* ��� $
��Auto �.+��� $��0������ $ DisplayImage��� $��1�� �� IsSelectionRangeEnabled . 
����  � � Value �� $ SliderValue�$ TwoWay=� � Minimum �� $ Minimum� � Maximum �� $ Maximum� �� SelectionStart �� $ AnalyseStart�$ TwoWay=� �� SelectionEnd �� $ 
AnalyseEnd�$ TwoWay=��� $��0��$��1��$��2q�y��� $ 	ShowVideo�# [ ���� $ DisplayText��� 
�������$��1q�$��2q�$ 2q�$��1��$��0��._�� y��� $ 	ShowVideo�# [  
 �� $
��Auto ��� $
��Auto ��� $
��Auto ��� $��* � �� $
��Auto ��� $��* ��� $
��Auto ��� $��* ��� $
��Auto ��� $��* ��� $
��Auto ��� $��* �.+��� $��Name: ��$��0��$��0���� $��0��$��1������ $
 Name��� $��
Distance: ��$��1��$��0���� $��1��$��1������ $ DistanceTravelled��� $��Centroid Distance: ��$��2��$��0���� $��2��$��1������ $ CentroidDistanceTravelled��� $��Start Frame: ��$��0��$��2���� $��1��$��3������ $ AnalyseStart��� $��End Frame: ��$��1��$��2���� $��2��$��3������ $ 
AnalyseEnd��� $��
Duration: ��$��2��$��2���� $��0��$��3������ $ Duration��� $��Average Velocity: ��$��0��$��4���� $��0��$��5������ $ AvgVelocity��� $!��Average Centroid Velocity: ��$��1��$��4���� $��1��$��5������ $ AvgCentroidVelocity��� $ ��Average Angular Velocity: ��$��2��$��4���� $��2��$��5������ $ AvgAngularVelocity��� $��Max Velocity: ��$��0��$��6���� $��0��$��7������ $ MaxVelocity��� $��Max Centroid Velocity: ��$��1��$��6���� $��1��$��7������ $ MaxCentroidVelocity��� $��Max Angular Velocity: ��$��2��$��6���� $��2��$��7������ $ MaxAngularVelocity��� $��3��$��0��$��8��$��2q����� $ SelectedMouseBehaviours� �� .P��� $��Boundary��$	 120�� �� *BoundaryName+�� $��Interaction��$	 120�� �� *Interaction+�� $��Frame��$ 50�� �� *FrameNumber+�� 
�������$��1q�$��2q�$ 2q�$��1��$��1��._�� y��� $ 	ShowVideo�# [  
 �� $
��Auto ��� $
��Auto ��� $
��Auto ��� $
��Auto ��� $
��Auto ��� $
��Auto ��� $
��Auto � �� $
��Auto ��� $��* �.+��� $��0��$��0��$��Gap Distance: ���� $��0��$��1�� �� $ GapDistance�$ TwoWay=� �� $ GapDistanceMin� �� $ GapDistanceMax�{��� $ VideoSelected��� $��1��$��0��$��Binary Treshold: ���� $��1��$��1�� �� $ BinaryThreshold�$ TwoWay=� �� $ BinaryThresholdMin� �� $ BinaryThresholdMax�{��� $ VideoSelected��� $��2��$��0��$��Binary Treshold 2: ���� $��2��$��1�� �� $ BinaryThreshold2�$ TwoWay=� �� $ BinaryThreshold2Min� �� $ BinaryThreshold2Max�{��� $ VideoSelected��� $��Smooth Motion: ��$��3��$��0���� $��3��$��1��$��Center=�$��2q� d� 	IsChecked �� $ SmoothMotion�$ TwoWay=��� $��Frame Rate: ��$��4��$��0���� $��4��$��1������ $ 	FrameRate�$ TwoWay=��� $��5��$��0��$��2�� �� Orientation$ 
Horizontal=�$��Center=�.+��� $��Center=�$��Start������ $ SetStartFrameCommand��� $��Center=�$	��End������ $ SetEndFrameCommand��� $��6��$��0��$��2��$ 
Horizontal=�$��Center=�.+��� $��Center=�$��Re-Run������ $ ReRunCommand��� $��Center=�$
��Save������ $ SaveCommand��� $��Center=�$��Export������ $ ExportCommand��� $��1��$ 
Horizontal=�$��Center=�.+��� $��Ok������ $ 	OkCommand��� $��Cancel������ $ CancelCommand�