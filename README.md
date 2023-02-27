
## 测试验证

### 测试数据
20000个customer，20000 * 10次upsert customer，100次随机by customerID的获取

### 测试场景
- 场景1：排序结果采用List，且写法不太干净
- 场景2：排序结果采用List
- 场景3：排序结果采用SortedSet
- 场景4：排序结果采用SortedSet，且基于某个Score得分值划分为两个集合（Part1、Part2）

**当前API采用的“场景3”**

![image](https://user-images.githubusercontent.com/8747775/221446851-6a1d6f45-31e4-4e50-8759-0be8f70cc33c.png)

### 测试记录（5轮测试的平均值）
|  场景1   | 场景2  | 场景3  | 场景4  |
|  ----  | ----  | ----  | ----  |
| 310秒  | 240秒 | 28秒 | 34秒 |

### 测试结论
- 20000Customer下采用List存储排序结果且对其检索，性能最差；
- 20000Customer下采用SortedSet存储排序结果且对其检索，性能比List提高了10倍，SortedSet在排序算法的时间复杂度上得到了体现；
- 20000Customer下采用SortedSet分区存储排序结果且对其检索，与直接采用单个SortedSet差异不大且有所降低，可能的原因是数据量还不够大&中间的Score值不太合理【后续待验证】；

### 某次的截图
![image](https://user-images.githubusercontent.com/8747775/221446922-a1b266a6-3e11-417c-a173-3bc26923e52d.png)


