   M S B A M L   `   `   `  ����Q  MAutomatedRodentTracker, Version=1.0.0.0, Culture=neutral, PublicKeyToken=nullU/clr-namespace:AutomatedRodentTracker.Behaviours!AutomatedRodentTracker.Behaviours  5   	   U/clr-namespace:AutomatedRodentTracker.Converters!AutomatedRodentTracker.Converters  5   	   @    :AutomatedRodentTracker.View.BatchProcess.Review.ReviewView   5   	   W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5   	   R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	   F
behaviours/clr-namespace:AutomatedRodentTracker.Behaviours     5   	   F
converters/clr-namespace:AutomatedRodentTracker.Converters     5   	     � Title$  
ReviewView��5   	   $	��480��6   $	��640��6)    � WindowState$ 	Maximized=�65   =   7AutomatedRodentTracker.Behaviours.WindowClosingBehavior   Close 5   	   ��  �� Path$ Close� �� Mode$ TwoWay=�   Closing 5   	   �� $ ClosingCommand� � 	Resources 5	      D   >AutomatedRodentTracker.Converters.BooleanToVisibilityConverter    BooleanToVisibilityConverterK   EAutomatedRodentTracker.Converters.InverseBooleanToVisibilityConverter ' #InverseBooleanToVisibilityConverter �� 
TargetType �� Property	 �� Value��%U  &	        &	      '�� &     '�� m     '�� �       5
   
   62     5   
   69   �� 5   
   " ��6   .�5      ��  � ��$	 2q�6'   5      �� 5   
   " ��6   .�5      ��  � ��$	 Center=�62   �� 5       � ��$	 1q�6'   5      �� 5   
   " ��6   .�5      ��  � ��$	 1q�6'   5      5      .��5      � 
 � RowDefinitions
 5   
   �� 5      $��* �6   �� 5      $
��Auto �6   .+�5   
   � $��0��6   
 5      �� 5      $��* �6    �� 5       $��* �6     � ColumnDefinitions 5"      �� 5#      $��* �6#   �� 5$      $��* �6#   .+�5'      �� $��0��6   $��0��6$   $��2q�6�   ��64   �� $ SingleFiles� �� SelectedItem 6]   �� $ SelectedVideo�$ TwoWay=� �� View 5(      �� 5)      .P�5*      �� $
��File��6)    �� Width$	 120��67    �� DisplayMemberBinding 6C   �� *VideoFileName+�� 5+      $��Result��6)   $ 50��69    6D   �� *VideoOutcome+�� 5,      $��Comments��6)   $	 150��6;    6G   �� *
Comments+5-      5/      �� 50      
�������$��1q�6(   $��2q�6<    �� Padding$ 2q�6G   $��0��6S   $��1��6a   ._�51      � $��2q�6v   y�6   �� $ 	ShowVideo� �� 	Converter# [  
 52      �� 53      $��* �6(   �� 54      $
��Auto �6(   .+�57      �� $��0��6   ��6)   �� $ DisplayImage��� 58      $��1��6    �� IsSelectionRangeEnabled . 
����  � � Value 6*   �� $ SliderValue�$ TwoWay=� � Minimum 6Z   �� $ Minimum� � Maximum 6{   �� $ Maximum� �� SelectionStart 6�   �� $ AnalyseStart�$ TwoWay=� �� SelectionEnd 6�   �� $ 
AnalyseEnd�$ TwoWay=�6+  59      5:      �� 5;      $��0��6   $��1��6&   $��2q�6�   y�66   �� $ 	ShowVideo�# [ ��6�   �� $ DisplayText�6�   �� 5=      
�������$��1q�6(   $��2q�6<   $ 2q�6G   $��1��6S   $��0��6`   ._�5>      � y�6   �� $ 	ShowVideo�# [  
 5?      �� 5@      $
��Auto �6(   �� 5A      $
��Auto �6(   �� 5B      $
��Auto �6(   �� 5C      $��* �6(    5E      �� 5F      $
��Auto �6+   �� 5G      $��* �6+   �� 5H      $
��Auto �6+   �� 5I      $��* �6+   �� 5J      $
��Auto �6+   �� 5K      $��* �6+   �� 5L      $
��Auto �6+   �� 5M      $��* �6+   .+�5P      �� $��Name: ��6    $��0��6.   $��0��6;   �� 5Q      $��0��6:   $��1��6G   ��6   �� $
 Name�6G   �� 5S      $��
Distance: ��6    $��1��62   $��0��6?   �� 5T      $��1��6G   $��1��6T   ��6   �� $ DistanceTravelled�6T   �� 5V      $��Centroid Distance: ��6    $��2��6;   $��0��6H   �� 5W      $��2��6O   $��1��6\   ��6   �� $ CentroidDistanceTravelled�6\   �� 5Y      $��Start Frame: ��6    $��0��65   $��2��6B   �� 5Z      $��1��6B   $��3��6O   ��6   �� $ AnalyseStart�6O   �� 5\      $��End Frame: ��6    $��1��63   $��2��6@   �� 5]      $��2��6@   $��3��6M   ��6   �� $ 
AnalyseEnd�6M   �� 5_      $��
Duration: ��6    $��2��62   $��2��6?   �� 5`      $��0��6>   $��3��6K   ��6   �� $ Duration�6K   �� 5b      $��Average Velocity: ��6    $��0��6:   $��4��6G   �� 5c      $��0��6A   $��5��6N   ��6   �� $ AvgVelocity�6N   �� 5e      $!��Average Centroid Velocity: ��6    $��1��6C   $��4��6P   �� 5f      $��1��6I   $��5��6V   ��6   �� $ AvgCentroidVelocity�6V   �� 5h      $ ��Average Angular Velocity: ��6    $��2��6B   $��4��6O   �� 5i      $��2��6H   $��5��6U   ��6   �� $ AvgAngularVelocity�6U   �� 5k      $��Max Velocity: ��6    $��0��66   $��6��6C   �� 5l      $��0��6A   $��7��6N   ��6   �� $ MaxVelocity�6N   �� 5n      $��Max Centroid Velocity: ��6    $��1��6?   $��6��6L   �� 5o      $��1��6I   $��7��6V   ��6   �� $ MaxCentroidVelocity�6V   �� 5q      $��Max Angular Velocity: ��6    $��2��6>   $��6��6K   �� 5r      $��2��6H   $��7��6U   ��6   �� $ MaxAngularVelocity�6U   �� 5t      $��3��6   $��0��6,   $��8��6<   $��2q�6�   ��6P   �� $ SelectedMouseBehaviours� 5u      �� 5v      .P�5w   "   �� $��Boundary��61   $	 120��6C    6O   �� *BoundaryName+�� 5x   "   $��Interaction��61   $	 120��6F    6R   �� *Interaction+�� 5y   "   $��Frame��61   $ 50��6@    6K   �� *FrameNumber+5z      5|      5}      5~      �� 5�      
�������$��1q�6(   $��2q�6<   $ 2q�6G   $��1��6S   $��1��6`   ._�5�      � y�6   �� $ 	ShowVideo�# [  
 5�      �� 5�      $
��Auto �6(   �� 5�      $
��Auto �6(   �� 5�      $
��Auto �6(   �� 5�      $
��Auto �6(   �� 5�      $
��Auto �6(   �� 5�      $
��Auto �6(   �� 5�      $
��Auto �6(    5�      �� 5�      $
��Auto �6+   �� 5�      $��* �6+   .+�5�      �� $��0��6    $��0��6-   $��Gap Distance: ��6=   �� 5�      $��0��6   $��1��6*    6:   �� $ GapDistance�$ TwoWay=� 6j   �� $ GapDistanceMin� 6�   �� $ GapDistanceMax�{�6�   �� $ VideoSelected��� 5�      $��1��6    $��0��6-   $��Binary Treshold: ��6=   �� 5�      $��1��6   $��1��6*    6:   �� $ BinaryThreshold�$ TwoWay=� 6n   �� $ BinaryThresholdMin� 6�   �� $ BinaryThresholdMax�{�6�   �� $ VideoSelected��� 5�      $��2��6    $��0��6-   $��Binary Treshold 2: ��6=   �� 5�      $��2��6   $��1��6*    6:   �� $ BinaryThreshold2�$ TwoWay=� 6o   �� $ BinaryThreshold2Min� 6�   �� $ BinaryThreshold2Max�{�6�   �� $ VideoSelected��� 5�      $��Smooth Motion: ��6    $��3��67   $��0��6D   �� 5�      $��3��6T   $��1��6a   $��Center=�6q   $��2q�6�    d� 	IsChecked 6   �� $ SmoothMotion�$ TwoWay=�6�   �� 5�      $��Frame Rate: ��6    $��4��64   $��0��6A   �� 5�      $��4��6K   $��1��6X   ��6   �� $ 	FrameRate�$ TwoWay=�6X   �� 5�      $��5��6!   $��0��6.   $��2��6>    �� Orientation$ 
Horizontal=�6R   $��Center=�6k   .+�5�      �� $��Center=�6!   $��Start��6>   ��6N   �� $ SetStartFrameCommand��� 5�      $��Center=�6!   $	��End��6>   ��6L   �� $ SetEndFrameCommand�5�      �� 5�      $��6��6!   $��0��6.   $��2��6>   $ 
Horizontal=�6R   $��Center=�6k   .+�5�      �� $��Center=�6!   $��Re-Run��6>   ��6O   �� $ ReRunCommand��� 5�      $��Center=�6!   $
��Save��6>   ��6M   �� $ SaveCommand��� 5�      $��Center=�6!   $��Export��6>   ��6O   �� $ ExportCommand�5�      5�      5�      5�      �� 5�   
   $��1��6   $ 
Horizontal=�6"   $��Center=�6;   .+�5�      �� $��Ok��6   ��6"   �� $ 	OkCommand��� 5�      $��Cancel��6   ��6&   �� $ CancelCommand�5�      5�      5�      