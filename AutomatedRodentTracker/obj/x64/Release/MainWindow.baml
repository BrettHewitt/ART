   M S B A M L   `   `   `  ���� Q  MAutomatedRodentTracker, Version=1.0.0.0, Culture=neutral, PublicKeyToken=nullU/clr-namespace:AutomatedRodentTracker.Behaviours!AutomatedRodentTracker.Behaviours  '    !AutomatedRodentTracker.MainWindow   W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     F
behaviours/clr-namespace:AutomatedRodentTracker.Behaviours       � Title$  Automated Rodent Tracker��$	��720��$
��1280�� � WindowState$ 	Maximized=�=   7AutomatedRodentTracker.Behaviours.WindowClosingBehavior   Closing ��  �� Path$ ClosingCommand� � 	Resources  �� 
TargetType �� Property �� Value��%�   '��       '�� O     �� " ��.���  � ��$	 100����  � ��$ 30����  � ��$ 5q��� " ��.���  � ��$ 3q� J� InputBindings �� 
	 �� Key$	 D��
 �� Command
 �� $ DisposeCommand�.���  � RowDefinitions �� $
��Auto ��� $��* ��� $
��Auto ��� $
��Auto �.+�o� $��0��.@�m� $
��File��.@�m� $	��New�� m� Command �� $ OpenVideoCommand�m� $
��Open�� �� $ OpenArtCommand�m� $
��Save�� �� $ SaveArtFileCommand�m� $��Batch Process�� �� $ BatchProcessCommand�m� $��Centroid Width�� �� $ BodyTestCommand�m� $��Validate�� �� $ ValidateCommand�� $��1�� � ColumnDefinitions �� $��* ��� $
��Auto �.+��� $��Center=�$��Center=�$��0������ $ Image��� $��0��$��Bottom=�$
��Left=�$��16(�
���  ��$��5q����� $ FrameNumberDisplay�� $��1�� �� $��* ��� $
��Auto �.+��� $��0��$��5q����� $ Events� �� View �� .P��� $��Action�� �� Width$	 120�� �� DisplayMemberBinding �� *Name+�� $��Object��$ 75�� �� *BoundaryName+�� $��Frame��$ 50�� �� *FrameNumber+�� $��1�� �� Orientation$ 
Horizontal=�$��Center=�.+��� $��Export Raw Data������ $ ExportRawDataCommand��� $��Export interactions������ $ ExportInteractionsCommand��� $��2��$��Stretch=� � Minimum$ 0Q� �� TickFrequency$ 1Q�$��5q� � Maximum �� $ 
FrameCount�{��� $ SliderEnabled� � Value �� $ CurrentFrame�