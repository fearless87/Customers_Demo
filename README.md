
## 测试验证

#### 测试结论
经过多轮测试后，得出“SortedSet改进后”比“原来List未改进”的方式对于如下的数据量下**快了8倍**左右

#### 测试数据
10000 * 10次upsert customer，100次随便by customerID的获取

#### 某次的截图
![image](https://user-images.githubusercontent.com/8747775/221334638-7545869a-cae4-4c8c-aab1-d5eb666f9d04.png)
