   M S B A M L   `   `   `  ����  CRM    CRM.SurveysListPage   5      R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5      R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5      Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5      @d2http://schemas.microsoft.com/expression/blend/2008     5        N� Title$!  Список опросов��5      .��5
      � $��10q�6    � RowDefinitions 5   
   �� 5      $
��Auto �6   �� 5      $��* �6   �� 5      $
��Auto �6   .+�5   
   �� $��0��6    �� Orientation$ 
Horizontal=�6"   $	��Top=�6;   .+�5      �� $��Фильтр:��6   $��Center=�6'   $��0,0,10,0q�6B   �� 5      -   5         ��Name$ StatusFilterComboBox��5      $	��150��65    �� SelectedIndex$ 0��6A   .@�5      �� $��Все опросы��6   �� 5      $#��Только активные��6   �� 5      $%��Только черновики��6   5      �� 5      -   5        $��Применить��5      $��10,0,0,0q�6)   $��80��6;   6F   �� 5      -   5        $��Сбросить��5      $��10,0,0,0q�6(   $��80��6:   6E   5      �� 5   
   $��1��6   $
��Auto=�6$   $
��0,10q�6G   .��5      &   System.Windows.Controls.DataGrid  -   5          Name$ SurveysDataGrid��5         AutoGenerateColumns .     CanUserAddRows .     SelectionMode$ Single=�5       $
��Auto��6.   	   Columns	 5!      0  *System.Windows.Controls.DataGridTextColumn  5"      ,  &System.Windows.Controls.DataGridColumn
   Header$
 ID��6)      Width5  /System.Windows.Controls.DataGridLengthConverter$ 50 6R   1  +System.Windows.Controls.DataGridBoundColumn   Binding 65   �� *
SurveyId+6R     5#      $
 Название��6)   $	 200 6U    6;   �� *Title+6U     5$      $
 Дата создания��6)   $	 120 6}    6@   �� *	CreatedAt+ �� StringFormat$ {0:dd.MM.yyyy}��6}   4  .System.Windows.Controls.DataGridCheckBoxColumn  5%      $
 Активный��6-   $ 80 6\    6?   �� *
IsActive+6\     5&      $
 Вопросов��6)   $ 80 6^    6;   �� *QuestionsCount+6^     5'      $
 Ответов��6)   $ 80 6]    6:   �� *ResponsesCount+6]   4  .System.Windows.Controls.DataGridTemplateColumn  5(      $
 Действия��6-   $	 300 6?      CellTemplate 5)      �� 5*      .R�5+   "   �� $ 
Horizontal=�6-   $��0q�6F   .+�5,   &   �� -   5        $��Просмотр��5,   -   $��70��5-      $	��2,0q�5.      $��2q�5/      
 � Tag 51      �� *
SurveyId+�� 52   &   -   5        $��
Отчет��52   -   $��60��53      $	��2,0q�54      $��2q�55       57      �� *
SurveyId+�� 58   &   -   5        $��Рассылка��58   -   $��75��59      $	��2,0q�5:      $��2q�5;       5=      �� *
SurveyId+5>   #   5?      5A      5C      5D      �� 5G   
   $��2��6   $ 
Horizontal=�6"   $��Right=�6;   $
��0,10q�6W   .+�5H      �� -   5        $��Новый опрос��5H      $	��120��6+   $��0,0,10,0q�67   6I   �� 5I      -	   5        $��Обновить��5I      $��80��6(   63   5J      5K      5L      