   M S B A M L   `   `   `  ���� Q  MAutomatedRodentTracker, Version=1.0.0.0, Culture=neutral, PublicKeyToken=nullU/clr-namespace:AutomatedRodentTracker.Behaviours!AutomatedRodentTracker.Behaviours  E    ?AutomatedRodentTracker.View.BatchProcess.Export.BatchExportView   W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     F
behaviours/clr-namespace:AutomatedRodentTracker.Behaviours       � Title$  BatchExportView��$	��300��$	��300��=   7AutomatedRodentTracker.Behaviours.WindowClosingBehavior   Close ��  �� Path$ Close� �� Mode$ TwoWay=� � 	Resources  �� 
TargetType �� Property �� Value��%�   '��       '�� Q     �� " ��.���  � ��$	 100����  � ��$ 5q���  � ��$
 Left=��� " ��.���  � ��$ 5q���  � ��$ Center=�.���  � RowDefinitions �� $��* ��� $
��Auto �.+�� $��0��	 � ColumnDefinitions	 �� $��* ��� $��* �.+��� $��0��
 �� DisplayMemberPath$

 Name��$��5q����� $ Videos�� $��1��	 �� $
��Auto ��� $��* � �� $
��Auto ��� $
��Auto ��� $
��Auto �.+��� $��0��$��0��$��
Velocity: ���� $��0��$��1�� �� Orientation$ 
Horizontal=�.+��� $

 Name������ $ VelocityOptions� �� SelectedItem �� $ SelectedVelocityOption�$ TwoWay=��� ���� $ BodyOptions� �� $ SelectedBodyOption�$ TwoWay=��� $��1��$��0��$��
Rotation: ���� $��1��$��1��$

 Name������ $ RotationOptions� �� $ SelectedRotationOption�$ TwoWay=��� $��1��$ 
Horizontal=�$��Center=�.+��� $��Ok������ $ 	OkCommand��� $��Process������ $ ProcessCommand�