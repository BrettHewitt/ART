   M S B A M L   `   `   `  ����Q  MAutomatedRodentTracker, Version=1.0.0.0, Culture=neutral, PublicKeyToken=nullU/clr-namespace:AutomatedRodentTracker.Behaviours!AutomatedRodentTracker.Behaviours  5   	   E    ?AutomatedRodentTracker.View.BatchProcess.Export.BatchExportView   5   	   W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5   	   R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	   F
behaviours/clr-namespace:AutomatedRodentTracker.Behaviours     5   	     � Title$  BatchExportView��5   	   $	��300��6!   $	��300��6.   =   7AutomatedRodentTracker.Behaviours.WindowClosingBehavior   Close 5   	   ��  �� Path$ Close� �� Mode$ TwoWay=� � 	Resources 5       �� 
TargetType �� Property �� Value��%  '��       '�� �     �� 5   
   " ��6   .�5	      ��  � ��$	 100��6&   �� 5
       � ��$ 5q�6'   �� 5       � ��$
 Left=�64   5      �� 5   
   " ��6   .�5      ��  � ��$ 5q�6'   �� 5       � ��$ Center=�62   5      5      .��5      �  � RowDefinitions 5   
   �� 5      $��* �6   �� 5      $
��Auto �6   .+�5   
   � $��0��6   	 � ColumnDefinitions	 5      �� 5      $��* �6#   �� 5      $��* �6#   .+�5      �� $��0��6   
 �� DisplayMemberPath$

 Name��6K   $��5q�6d   ��6'   �� $ Videos�5       � 5"      $��1��6   	 5#      �� 5$      $
��Auto �6'   �� 5%      $��* �6'    5'      �� 5(      $
��Auto �6$   �� 5)      $
��Auto �6$   �� 5*      $
��Auto �6$   .+�5-      �� $��0��6   $��0��6)   $��
Velocity: ��69   �� 5.      $��0��6   $��1��6*    �� Orientation$ 
Horizontal=�6:   .+�5/      �� $

 Name��6�   ��6   �� $ VelocityOptions� �� SelectedItem 6L   �� $ SelectedVelocityOption�$ TwoWay=�6�   �� 50      ��6   �� $ BodyOptions� 6H   �� $ SelectedBodyOption�$ TwoWay=�51      �� 54      $��1��6   $��0��6)   $��
Rotation: ��69   �� 55      $��1��6   $��1��6(   $

 Name��6�   ��68   �� $ RotationOptions� 6e   �� $ SelectedRotationOption�$ TwoWay=�6�   58      59      �� 5;   
   $��1��6   $ 
Horizontal=�6"   $��Center=�6;   .+�5<      �� $��Ok��6   ��6"   �� $ 	OkCommand��� 5=      $��Process��6   ��6'   �� $ ProcessCommand�5>      5?      5@      