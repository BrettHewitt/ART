   M S B A M L   `   `   `  ����   AutomatedRodentTracker;    5AutomatedRodentTracker.View.HoughLinesTest.HoughLines   R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml       � Title$  Hough Lines��$	��720��$
��1280�� � WindowState$ 	Maximized=� � 	Resources  �� 
TargetType �� Property �� Value��%,   '��       �� " ��.���  � ��$ 5q�.���  � RowDefinitions �� $
��Auto ��� $��* ��� $��* ��� $��* ��� $
��Auto ��� $
��Auto ��� $
��Auto ��� $
��Auto ��� $
��Auto � � ColumnDefinitions �� $
��Auto ��� $��* �.+�o� $��0��$��0��$��2��.@�m� $
��File��.@�m� $
��Open�� m� Command �� 	 �� Path$	 OpenCommand�m� $
��Save�� �� $	 SaveCommand�� $��1��$��3��$��0��$��2�� �� $��* ��� $��* ��� $��* �.+��� $��0������ $	 DisplayImage��� $��1������ $	 DisplayImage2��� $��2������ $	 DisplayImage3��� $��Distance Res: ��$��4��$��0���� $��Angle Res: ��$��5��$��0���� $��Threshold: ��$��6��$��0���� $��Min Line Width: ��$��7��$��0���� $��Gap Between Lines: ��$��8��$��0���� $��4��$��1��
 � Minimum$	
 0.1Q� � Maximum$	 100Q� �� TickFrequency$	 0.1Q� � Value �� $	 DistanceResolution� �� UpdateSourceTrigger$ PropertyChanged=��� $��5��$��1��$
 5Q�$	 180Q�$	 0.1Q� �� $	 AngleResolution�$ PropertyChanged=��� $��6��$��1��$
 1Q�$	 200Q�$ 1Q� �� $	 	Threshold�$ PropertyChanged=��� $��7��$��1��$	
 0.1Q�$ 50Q�$	 0.1Q� �� $	 MinLineWidth�$ PropertyChanged=��� $��8��$��1��$
 1Q�$	 100Q�$ 1Q� �� $	 GapBetweenLines�$ PropertyChanged=�