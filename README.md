knn-on-air
==========
### [KNNonAir.exe] GUI工具，產出實驗用檔案

【Step 1】 設定參數，依序為 `Location` `Algorithm` `Number of Regions` `Packet Size` `Value of K`

![](https://dl.dropboxusercontent.com/u/16991848/README/parameter.PNG "Set Parameter")

預設為 GU(關島)、EB (VEB演算法)、16個區塊、128 Bytes、10NN，實驗設定參數如下(排列組合)，

| | 參數 |
|---:|:---| 
| Location | GU、NV、WA、MS、FL、NC、TX、CA |
| Algorithm | EB (VEB) 演算法、PA (BNI) 演算法、NPI 演算法 |
| Number of Regions | 1、4、16、64、256 |

【Step 2】 ![](https://dl.dropboxusercontent.com/u/16991848/README/add.png "Read File") `Add Roads` 讀取 *.geojson 道路圖資

[註] 綠色 Marker 表示斷掉的線段端點，__用滑鼠滾輪縮放地圖後才會顯示計算結果！__

![](https://dl.dropboxusercontent.com/u/16991848/README/add_roads.PNG "Add Roads")

【Step 3】 ![](https://dl.dropboxusercontent.com/u/16991848/README/add.png "Read File") `Add PoIs` 讀取 *.geojson 地標圖資

[註] 此時綠色 Marker 表示道路上的物件

![](https://dl.dropboxusercontent.com/u/16991848/README/add_pois.PNG "Add PoIs")

【Step 4】 ![](https://dl.dropboxusercontent.com/u/16991848/README/save.png "Save File") `Save Roads and PoIs` 儲存道路和地標，之後可透過 ![](https://dl.dropboxusercontent.com/u/16991848/README/add.png "Read File") `Add Roads and PoIs` 直接跳過 Step 2, Step 3。

【Step 5】 ![](https://dl.dropboxusercontent.com/u/16991848/README/shortcut.png "Generate NVD") `Generate NVD` 產生 NVD

![](https://dl.dropboxusercontent.com/u/16991848/README/nvd.png "Generate NVD")

【Step 6】 ![](https://dl.dropboxusercontent.com/u/16991848/README/save.png "Save File") `Save Roads and PoIs` 儲存道路和地標，之後可透過 ![](https://dl.dropboxusercontent.com/u/16991848/README/add.png "Read File") `Add NVD` 直接跳過 Step 2, Step 3, Step 4, Step 5， NPI 不需要 NVD 所以只能跳過 Step 2, Step 3。

[註] __NPI 演算法不需要產生 NVD，但是 EB 和 PA 需要。__

【Step 7】 ![](https://dl.dropboxusercontent.com/u/16991848/README/grid.png "Partition") `Partition` 切割道路區塊 (1、4、16、64、256)

eg. 切割16區塊，圖中省略了 Marker

![](https://dl.dropboxusercontent.com/u/16991848/README/partiton.PNG "Partition")

【Step 8】 ![](https://dl.dropboxusercontent.com/u/16991848/README/index.png "Generate Index") `Generate Index`  計算索引結構，不同演算法的索引結構各不相同，其中 PA 的索引結構不會顯示在畫面上。

![](https://dl.dropboxusercontent.com/u/16991848/README/generate_index.PNG "Generate Index")

【Step 9】 ![](https://dl.dropboxusercontent.com/u/16991848/README/table.png "Generate Table") `Generate Table`  計算各種數量(1、4、16、64、256)的區塊間的距離資訊

![](https://dl.dropboxusercontent.com/u/16991848/README/ebtable.PNG "Generate Table")

【Step 10】 ![](https://dl.dropboxusercontent.com/u/16991848/README/save.png "Save File") `Save EB Table` 或 `Save NPI Table` 儲存距離資訊，之後可透過 ![](https://dl.dropboxusercontent.com/u/16991848/README/add.png "Read File") `Add EB Table` 或 `Add NPI Table` 直接跳過 Step 9，而 PA 需要執行 Step 9 但是不需要儲存距離資訊。

【Step 11】 ![](https://dl.dropboxusercontent.com/u/16991848/README/search.png "kNN Search") `kNN Search` 搜尋 kNN

![](https://dl.dropboxusercontent.com/u/16991848/README/knn.PNG "kNN Search")

==========
### [Evaluation.exe] 直接點只會閃過一瞬間的視窗，要用 *.bat 跑實驗
實驗設定參數如下(排列組合)，

| | 參數 |
|---:|:---| 
| Location | GU、NV、WA、MS、FL、NC、TX、CA |
| Algorithm | VEB 演算法、BNI 演算法、NPI 演算法 |
| Number of Regions | 1、4、16、64、256 |
| Packet Size | 128、256、384、512、640、768、896、1024 |
| Value of K | 1、5、10、15、20、25、30 |

eg. *.bat EB 範例: 執行檔位置、NVD 檔案位置、切割區塊數量、使用的演算法、對應切割區塊數量的 ebtable 檔案位置、 k值、封包大小

* start Evaluation.exe GU_nvd.xml 4 EB GU_4_ebtable.json 10 128

eg. *.bat PA 範例: 執行檔位置、NVD 檔案位置、切割區塊數量、使用的演算法、 k值、封包大小

* start Evaluation.exe NV_nvd.xml 64 PA 15 128

eg. *.bat NPI 範例: 執行檔位置、road_pois 檔案位置、切割區塊數量、使用的演算法、對應切割區塊數量的 npitable 檔案位置、 k值、封包大小

* start Evaluation.exe WA_road_pois.json 16 NPI WA_16_npitable.json 10 640

實驗結束會出現 Finish!，並產生對應的 txt 實驗結果檔。

![](https://dl.dropboxusercontent.com/u/16991848/README/finish.PNG "Finish!")
