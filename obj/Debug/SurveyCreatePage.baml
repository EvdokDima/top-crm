   M S B A M L   `   `   `  ����  CRM    CRM.SurveyCreatePage   5      R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5      R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5      Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5      @d2http://schemas.microsoft.com/expression/blend/2008     5        N� Title$#  Создание опроса��5      .��5
      � $��10q�6    � RowDefinitions 5   
   �� 5      $
��Auto �6   �� 5      $��* �6   �� 5      $
��Auto �6   .+�5   
   �� $��0��6   .+�5      ��  �� Orientation$ 
Horizontal=�6   $	��0 5q�62   .+�5      �� $$��Название опроса:��6   $	��150��64   $��Center=�6@   �� 5      -   5         ��Name$ TitleTextBox��5      $	��400��60   5      �� 5      $ 
Horizontal=�6   $	��0 5q�62   .+�5      �� $��Описание:��6   $	��150��6-   $	��Top=�69   �� 5      -   5        $ DescriptionTextBox��5      $	��400��66   $��60��6B    �� TextWrapping$
 Wrap=�5       �� AcceptsReturn . 6.   5      �� 5      -   5         ��Name$ IsActiveCheckBox��5      $!��Активный опрос��61   $
��0 10q�6J    d� 	IsChecked$
 TrueY�6X   5      �� 5"   
   $��1��6   $
��Auto=�6$   .��5#      �� -   5         ��Name$ QuestionsStackPanel��5#      $
��0 10q�66   5$      �� 5'   
   $��2��6   $ 
Horizontal=�6"   $��Right=�6;   $
��0 10q�6W   .+�5(      �� -   5        	 ��Name$	 
ListButton��5(      $!��Список опросов��6)   $	��150��6B   $��0 0 10 0q�6N   6`   �� 5)      -   5        $#��Добавить вопрос��5)      $	��150��6/   $��0 0 10 0q�6;   6M   �� 5*      -   5        $#��Сохранить опрос��5*      $	��150��6/   6;   5+      5,      5-      