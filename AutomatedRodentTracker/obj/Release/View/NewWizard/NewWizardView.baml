   M S B A M L   `   `   `  ���� Q  MAutomatedRodentTracker, Version=1.0.0.0, Culture=neutral, PublicKeyToken=nullU/clr-namespace:AutomatedRodentTracker.Behaviours!AutomatedRodentTracker.Behaviours  9    3AutomatedRodentTracker.View.NewWizard.NewWizardView   R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     F
behaviours/clr-namespace:AutomatedRodentTracker.Behaviours       � Title$  Preview��$
��1280��$	��720�� � WindowState$ 	Maximized=�=   7AutomatedRodentTracker.Behaviours.WindowClosingBehavior   Close ��  �� Path$ Close�   Closing �� $ ClosingCommand� � 	Resources  �� 
TargetType �� Property �� Value��%^   '��       '�� "     �� " ��.���  � ��$ 5q��� " ��.���  � ��$ Stretch=�.��� 	 � RowDefinitions	 �� $��* ��� $
��Auto �.+�� $��0��
 � ColumnDefinitions
 �� $��* ��� $��* �.+�� $��0��	 �� $��* ��� $��* �.+�� $��0��	 �� $
��Auto ��� $
��Auto ��� $
��Auto ��� $
��Auto ��� $
��Auto ��� $
��Auto �
 �� $
��Auto ��� $��* �.+��� $��Frame: ��$��0��$��0���� $��0��$��1��$��5q� � Value �� $ FrameNumber� � Maximum �� $ MaxFrame��� $��ROI: ��$��1��$��0����  �� Orientation$ 
Horizontal=�$��1��$��1��.+��� $	��Set������ $ SetRoiCommand��� $��Remove������ $ RemoveRoiCommand��� $
��Save������ $ TestCommand��� $��Binary Threshold: ��$��2��$��0���� $��2��$��1�� � Minimum$ 0Q�$��5q� �� $ MaxThreshold� �� $ ThresholdValue� �� Mode$ TwoWay=��� $��Gap Distance: ��$��3��$��0���� $��3��$��1��$ 0Q�$	 500Q�$��5q� �� $ GapDistance�$ TwoWay=��� $��Interaction Threshold: ��$��4��$��0���� $��4��$��1��$��5q�$��40��$
��Left=����� $ InteractionValue�$ TwoWay=��� $��Motion Length: ��$��5��$��0���� $��5��$��1��$��5q�$��40��$
��Left=����� $ MotionLength�$ TwoWay=��� $��1��$��5q����� $ 	Boundries� �� View �� .P��� $
��Name�� �� Width$	 120�� �� CellTemplate �� .R��� $��5q�$��Center=�$��Stretch=����� $
 Name�$ TwoWay=��� $��Enabled��$ 50�� �� .R��� $��5q�$��Center=� d� 	IsChecked �� $ Enabled�$ TwoWay=�� $��1��	 �� $��* �.+��� $��0������ $ DisplayImage��� $��1��$ 
Horizontal=�$��Center=�.+��� $��Preview������ $ PreviewCommand��� $��Ok������ $ 	OkCommand��� $��Cancel������ $ CancelCommand�