   M S B A M L   `   `   `  ����Q  MAutomatedRodentTracker, Version=1.0.0.0, Culture=neutral, PublicKeyToken=nulle7clr-namespace:AutomatedRodentTracker.View.BodyDetection)AutomatedRodentTracker.View.BodyDetection  5   	   A    ;AutomatedRodentTracker.View.BodyDetection.BodyDetectionView   5   	   W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5   	   R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	   @d2http://schemas.microsoft.com/expression/blend/2008     5   	   Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5   	   Ilocal7clr-namespace:AutomatedRodentTracker.View.BodyDetection     5   	     � Title$  BodyDetectionView��5   	   $	��720��6#   $
��1280��60    � WindowState$ 	Maximized=�6=   .��5	      �  � RowDefinitions 5
   
   �� 5      $
��Auto �6   �� 5      $��* �6   �� 5      $
��Auto �6   �� 5      $
��Auto �6   .+�5   
   o� $��0��6   .@�5      m� $
��File��6   .@�5      m� $
��Test��6   5      5      � 5   
   $��1��6    5      �� 5      $��* �6    �� 5      $��* �6     � ColumnDefinitions 5      �� 5      $��* �6#   �� 5      $��* �6#   �� 5      $��* �6#   .+�5"      �� $��0��6   $��0��6!   $��5q�6P   ��61   ��  �� Path$ Image1�6P   �� 5#      $��0��6   $��1��6!   $��5q�6P   ��61   �� $ Image2�6P   �� 5$      $��0��6   $��2��6!   $��5q�6P   ��61   �� $ Image3�6P   �� 5%      $��1��6   $��0��6!   $��5q�6P   ��61   �� $ Image4�6P   �� 5&      $��1��6   $��1��6!   $��5q�6P   ��61   �� $ Image5�6P   �� 5'      $��1��6   $��2��6!   $��5q�6P   ��61   �� $ Image6�6P   5)      �� 5+   
   $��2��6    �� Orientation$ 
Horizontal=�6"   $��Center=�6;   .+�5,      �� $��Previous��6   ��6(   �� $ PreviousCommand��� 5-      $
��Next��6   ��6$   �� $ NextCommand��� 5.      $��	Save Skel��6   ��6)   �� $ SaveSkelCommand�5/      �� 51   
   $��3��6    � Minimum$ 0Q�6A    �� TickFrequency$ 1Q�6t   $��5q�6�    � Value 6   �� $ SliderValue�	 � Maximum	 6M   �� $ SliderMaximum�6�   53      54      