   M S B A M L   `   `   `  ����Q  MAutomatedRodentTracker, Version=1.0.0.0, Culture=neutral, PublicKeyToken=nullU/clr-namespace:AutomatedRodentTracker.Behaviours!AutomatedRodentTracker.Behaviours  5   	   9    3AutomatedRodentTracker.View.NewWizard.NewWizardView   5   	   W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5   	   R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	   F
behaviours/clr-namespace:AutomatedRodentTracker.Behaviours     5   	     � Title$  Preview��5   	   $
��1280��6   $	��720��6&    � WindowState$ 	Maximized=�63   =   7AutomatedRodentTracker.Behaviours.WindowClosingBehavior   Close 5   	   ��  �� Path$ Close�   Closing 5   	   �� $ ClosingCommand� � 	Resources 5       �� 
TargetType �� Property �� Value��%�   '��       '�� G     �� 5	   
   " ��6   .�5
      ��  � ��$ 5q�6'   5      �� 5   
   " ��6   .�5      ��  � ��$ Stretch=�6;   5      5      .��5      � 	 � RowDefinitions	 5   
   �� 5      $��* �6   �� 5      $
��Auto �6   .+�5   
   � $��0��6   
 � ColumnDefinitions
 5      �� 5      $��* �6#   �� 5      $��* �6#   .+�5      � $��0��6   	 5      �� 5      $��* �6$   �� 5      $��* �6$   .+�5"      � $��0��6   	 5#      �� 5$      $
��Auto �6(   �� 5%      $
��Auto �6(   �� 5&      $
��Auto �6(   �� 5'      $
��Auto �6(   �� 5(      $
��Auto �6(   �� 5)      $
��Auto �6(   
 5+      �� 5,      $
��Auto �6+   �� 5-      $��* �6+   .+�50      �� $��Frame: ��6    $��0��6/   $��0��6<   �� 51      $��0��6   $��1��6*   $��5q�6    � Value 6:   �� $ FrameNumber� � Maximum 6]   �� $ MaxFrame�6   �� 53      $��ROI: ��6    $��1��6-   $��0��6:   �� 54       �� Orientation$ 
Horizontal=�6!   $��1��6:   $��1��6G   .+�55      �� $	��Set��6!   ��6/   �� $ SetRoiCommand��� 56      $��Remove��6!   ��62   �� $ RemoveRoiCommand��� 57      $
��Save��6!   ��60   �� $ TestCommand�58      �� 5:      $��Binary Threshold: ��6    $��2��6:   $��0��6G   �� 5;      $��2��6   $��1��6*    � Minimum$ 0Q�6:   $��5q�6�    6F   �� $ MaxThreshold� 6l   �� $ ThresholdValue� �� Mode$ TwoWay=�6�   �� 5=      $��Gap Distance: ��6    $��3��66   $��0��6C   �� 5>      $��3��6   $��1��6*   $ 0Q�6;   $	 500Q�6G   $��5q�6�    6U   �� $ GapDistance�$ TwoWay=�6�   �� 5@      $��Interaction Threshold: ��6    $��4��6?   $��0��6L   �� 5A      $��4��6   $��1��6+   $��5q�6o   $��40��6z   $
��Left=�6�   ��6;   �� $ InteractionValue�$ TwoWay=�6�   �� 5C      $��Motion Length: ��6    $��5��67   $��0��6D   �� 5D      $��5��6   $��1��6+   $��5q�6k   $��40��6v   $
��Left=�6�   ��6;   �� $ MotionLength�$ TwoWay=�6�   5F      �� 5H      $��1��6   $��5q�6O   ��6(   �� $ 	Boundries� �� View 5I      �� 5J      .P�5K      �� $
��Name��6-    �� Width$	 120��6;    �� CellTemplate 5L   "   �� 5M   &   .R�5N   *   �� $��5q�6Z   $��Center=�6e   $��Stretch=�6�   ��62   �� $
 Name�$ TwoWay=�6�   5O   '   5Q      �� 5R      $��Enabled��6-   $ 50��6>    5S   "   �� 5T   &   .R�5U   *   �� $��5q�6c   $��Center=�6n    d� 	IsChecked 63   �� $ Enabled�$ TwoWay=�6n   5V   '   5X      5Y      5[      5]      � 5_      $��1��6   	 5`      �� 5a      $��* �6$   .+�5d      �� $��0��6   ��6%   �� $ DisplayImage�5e      5f      �� 5h   
   $��1��6   $ 
Horizontal=�6"   $��Center=�6;   .+�5i      �� $��Preview��6   ��6'   �� $ PreviewCommand��� 5j      $��Ok��6   ��6"   �� $ 	OkCommand��� 5k      $��Cancel��6   ��6&   �� $ CancelCommand�5l      5n      5o      