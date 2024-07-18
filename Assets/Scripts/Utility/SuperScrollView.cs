using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;


public class SuperScrollView : MonoBehaviour, IBeginDragHandler, IEndDragHandler
{
    private ScrollRect mScrollRect;
    private RectTransform mContentRect;
    private float targetVertical = 0;               //滑动的目标坐标
    private bool isInSlide = false;                     //是否在滑动中
    private bool isDragEnd = false;                    //是否拖拽结束
    private List<float> posList = new List<float>();//求出每页的临界角，页索引从0开始
    private int currentPageIndex = 0;
    private bool stopMove = true;
    private float startTime;
    private float prePosY = -1;          //之前Y轴的位置
    public float smooting = 4;      //滑动速度
    public bool isAdsorb = false;   //是否吸附

    public GameObject itemPrefab;
    public float itemHeight;//item的高度（全都一样）
    public float ItemWidth;
    public int maxItemCount; //最大item生成数量，真实item生成数量的阀值
    public int datasNum;//数据数量
    private int currentItemCount;//已生成的item的数量
    public Action<GameObject, int, bool> refreshItem;//item刷新函数


    public int rowCount;    //一排的数量
    public int beginOffset; //起始偏移量

    private float MoveY = 0;
    private bool IsGoTo = false;
    private bool IsSlidePosEqual = false;

    public int initIndex;//设置初始序号
    public bool isUpperStart;//是否从上往下排列

    //记录最上和最下的item索引
    private int firstIndex;
    private int lastIndex;
    private List<GameObject> itemList = new List<GameObject>();

    private float content_pos;
    private int pre_first_index;
    private int pre_last_index;

    // Use this for initialization
    public void Init(int sum, Action<GameObject, int, bool> callback)
    {
        datasNum    = sum;
        refreshItem = callback;
        initIndex   = 0;


        mScrollRect = transform.GetComponent<ScrollRect>();
        mContentRect = mScrollRect.content;//.transform.GetComponent<RectTransform>();
        mScrollRect.onValueChanged.AddListener((Vector2 vec) => OnScrollMove(vec));

        InitScroll();

        float verticalLength = mScrollRect.content.rect.height - GetComponent<RectTransform>().rect.height;
        if (isUpperStart)
        {
            posList.Add(1);
            for (int i = 1; i < datasNum - 1; i++)
            {
                float value = 1 - itemHeight * i / verticalLength;
                if (value > 0)
                {
                    posList.Add(value);
                }
            }
        }
    }

    public void InitScroll()
    {
        for (int i = 0; i < itemList.Count; i++)
        {
            GameObject item = itemList[i];
            Destroy(item);
        }
        itemList.Clear();
        SetScroller();
    }

    public void SetScroller()
    {
        SetContentHeight();
        InitCountent();
    }

    //设置滚动条的Content高度
    public void SetContentHeight()
    {
        int realHeight = (int)Mathf.Ceil(datasNum / (float)rowCount);
        mContentRect.sizeDelta = new Vector2(mContentRect.sizeDelta.x, itemHeight * realHeight + Mathf.Abs(beginOffset));
    }


    //初始化生成固定数量的Item
    public void InitCountent()
    {
        int needItem = Mathf.Clamp(datasNum, 0, maxItemCount);
        for (int i = initIndex; i < needItem + initIndex; i++)
        {
            GameObject _obj = Instantiate(itemPrefab);
            _obj.transform.SetParent(mContentRect.transform);
            _obj.transform.localScale = new Vector3(1, 1, 1);
            _obj.name = i.ToString();
            refreshItem.Invoke(_obj, i, true);


            int realRow = i % (rowCount);
            int realHeight = (int)Mathf.Floor(i / rowCount);
            float posX = (float)((realRow + 1) - rowCount / 2.0 - 0.5) * ItemWidth;

            RectTransform _rectTrans = _obj.GetComponent<RectTransform>();
            if (isUpperStart == true)
            {
                _rectTrans.pivot = new Vector2(0.5f, 1);
                _rectTrans.anchorMax = new Vector2(0.5f, 1);
                _rectTrans.anchorMin = new Vector2(0.5f, 1);
                _rectTrans.anchoredPosition = new Vector2(posX, -beginOffset - itemHeight * realHeight);
            }
            else
            {
                _rectTrans.pivot = new Vector2(0.5f, 0);
                _rectTrans.anchorMax = new Vector2(0.5f, 0);
                _rectTrans.anchorMin = new Vector2(0.5f, 0);
                _rectTrans.anchoredPosition = new Vector2(posX, beginOffset + itemHeight * realHeight);
            }
            
            currentItemCount += 1;
            lastIndex = i;
            itemList.Add(_obj);
        }

        firstIndex = initIndex;

        if (isUpperStart == true)
        {
            mContentRect.transform.localPosition = new Vector3(0, itemHeight * initIndex, 0);
        }
        else
        {
            mContentRect.transform.localPosition = new Vector3(0, -itemHeight * initIndex, 0);
        }
    }

    public void OnScrollMove(Vector2 pVec)
    {
        if (IsGoTo == true)
        {
            MoveY = pVec.y;
            IsGoTo = false;
            return;
        }

        if (isUpperStart == true)
        {
            //向上滚动
            int Height = (int)Mathf.Floor(firstIndex / rowCount);
            int lastHeight = (int)Mathf.Floor(lastIndex / rowCount);
            int totalHeight = (int)Mathf.Floor(maxItemCount / rowCount);

            if ((MoveY > pVec.y) && mContentRect.anchoredPosition.y >= ((Height + 1) * itemHeight + beginOffset) && lastIndex != datasNum - 1)
            {
                int addRow = (int)Mathf.Ceil((mContentRect.anchoredPosition.y - ((Height + 1) * itemHeight + beginOffset)) / itemHeight);
                int real_count = rowCount * addRow;
                for (int i = 0; i < real_count; i++)
                {
                    if (lastIndex >= datasNum - 1)
                    {
                        break;
                    }
                    //思路是List中的保存的GameObject顺序与真实显示的物体保持一致
                    GameObject _first = itemList[0];
                    if (_first != null)
                    {
                        RectTransform _rectTrans = _first.GetComponent<RectTransform>();
                        //将首个物体移出List，再添加到List最后端
                        itemList.RemoveAt(0);
                        itemList.Add(_first);

                        firstIndex += 1;
                        lastIndex += 1;

                        int realRow = lastIndex % (rowCount);
                        int realHeight = (int)Mathf.Floor(lastIndex / rowCount);
                        float posX = (float)((realRow + 1) - rowCount / 2.0 - 0.5) * ItemWidth;
                        float posY = -realHeight * itemHeight - beginOffset;

                        //Debug.Log("测试输出 高度index：" + realHeight);

                        //将这个物体移到最下方
                        _rectTrans.anchoredPosition = new Vector2(posX, posY);
                        //Debug.Log("测试输出 序列：" + lastIndex + ", x：" + posX + ", y:" + posY);

                        //修改显示
                        _first.name = lastIndex.ToString();
                        refreshItem.Invoke(_first, lastIndex, false);
                    }
                }
            }

            //向下滚动
            if ((MoveY < pVec.y) && mContentRect.anchoredPosition.y <= ((lastHeight + 1 - totalHeight) * itemHeight + beginOffset) && firstIndex > 0)
            {
                int addRow = (int)Mathf.Ceil((((lastHeight + 1 - totalHeight) * itemHeight + beginOffset) - mContentRect.anchoredPosition.y) / itemHeight);
                int real_count = Mathf.Min(rowCount, (firstIndex - 1) % rowCount + 1);
                if (addRow > 1)
                {
                    real_count += rowCount * (addRow - 1);
                }
                for (int i = 0; i < real_count; i++)
                {
                    if (firstIndex <= 0)
                    {
                        break;
                    }
                    GameObject _last = itemList[itemList.Count - 1];
                    if (_last != null)
                    {
                        RectTransform _rectTrans = _last.GetComponent<RectTransform>();
                        itemList.RemoveAt(itemList.Count - 1);
                        itemList.Insert(0, _last);

                        firstIndex -= 1;
                        lastIndex -= 1;

                        int realRow = firstIndex % (rowCount);
                        int realHeight = (int)Mathf.Floor(firstIndex / rowCount);
                        float posX = (float)((realRow + 1) - rowCount / 2.0 - 0.5) * ItemWidth;
                        float posY = -realHeight * itemHeight - beginOffset;

                        //将这个物体移到最下方
                        _rectTrans.anchoredPosition = new Vector2(posX, posY);

                        _last.name = firstIndex.ToString();
                        refreshItem.Invoke(_last, firstIndex, false);
                    }
                }
            }
        }

        MoveY = pVec.y;
    }

    private void Update()
    {
        float offset_posy = 0.001f;
        bool isSlideEnd = false;
        if (isInSlide == true && IsSlidePosEqual == true && Mathf.Abs(mScrollRect.verticalNormalizedPosition - prePosY) <= offset_posy)
        {
            isSlideEnd = true;
            IsSlidePosEqual = false;
        }
        IsSlidePosEqual = Mathf.Abs(mScrollRect.verticalNormalizedPosition - prePosY) <= offset_posy;
        prePosY = mScrollRect.verticalNormalizedPosition;

        //计算当前离第几行最近
        float posY = mScrollRect.verticalNormalizedPosition;
        posY = posY < 1 ? posY : 1;
        posY = posY > 0 ? posY : 0;
        int index = 0;
        float offset = Mathf.Abs(posList[index] - posY);
        for (int i = 1; i < posList.Count; i++)
        {
            float temp = Mathf.Abs(posList[i] - posY);
            if (temp < offset)
            {
                index = i;
                offset = temp;
            }
        }

        content_pos = posY;
        pre_first_index = firstIndex;
        pre_last_index = lastIndex;
        SetPageIndex(index);

        if (isAdsorb && isDragEnd && isSlideEnd && stopMove == true)
        {
            targetVertical = posList[currentPageIndex]; //设置目标坐标，更新函数进行插值
            startTime = 0;
            stopMove = false;
            isInSlide = false;
        }

        if (!stopMove)
        {
            startTime += Time.deltaTime;
            float t = startTime * smooting;
            mScrollRect.verticalNormalizedPosition = Mathf.Lerp(mScrollRect.verticalNormalizedPosition, targetVertical, t);
            if (t >= 1)
            {
                stopMove = true;
                isDragEnd = false;
                mScrollRect.verticalNormalizedPosition = targetVertical;
            }
        }
    }

    public void AdsorbToPage(int target_index)
    {
        SetPageIndex(target_index);
        targetVertical = posList[target_index]; //设置目标坐标，更新函数进行插值
        startTime = 0;
        stopMove = false;
        isInSlide = false;
    }

    public void AdsorbLast(int target_index)
    {
        //Debug.Log("测试输出 AdsorbToPage ： " + content_pos);
        SetPageIndex(target_index);

        IsGoTo = true;
        firstIndex = pre_first_index;
        lastIndex = pre_last_index;


        mScrollRect.verticalNormalizedPosition = content_pos;
        for (int i = 0; i <= maxItemCount - 1; i++)
        {
            if (i >= itemList.Count)
            {
                break;
            }
            GameObject _obj = itemList[i];
            int index = i + firstIndex;
            _obj.name = index.ToString();
            refreshItem.Invoke(_obj, index, true);


            int realRow = index % (rowCount);
            int realHeight = (int)Mathf.Floor(index / rowCount);
            float posX = (float)((realRow + 1) - rowCount / 2.0 - 0.5) * ItemWidth;
            float posY = -realHeight * itemHeight - beginOffset;

            //将这个物体移到最下方
            RectTransform _rectTrans = _obj.GetComponent<RectTransform>();
            _rectTrans.anchoredPosition = new Vector2(posX, posY);
        }
    }

    public void GoToPage(int init_index, int offest)
    {
        SetPageIndex(init_index);
        IsGoTo = true;
        firstIndex = Mathf.Max(init_index + offest, 0);
        lastIndex = firstIndex + maxItemCount - 1;
        mScrollRect.verticalNormalizedPosition = posList[init_index];
        for (int i = 0; i <= maxItemCount - 1; i++)
        {
            GameObject _obj = itemList[i];
            int index = i + firstIndex;
            _obj.name = index.ToString();
            refreshItem.Invoke(_obj, index, true);
            int realRow = index % (rowCount);
            int realHeight = (int)Mathf.Floor(index / rowCount);
            float posX = (float)((realRow + 1) - rowCount / 2.0 - 0.5) * ItemWidth;

            RectTransform _rectTrans = _obj.GetComponent<RectTransform>();
            if (isUpperStart == true)
            {
                _rectTrans.anchoredPosition = new Vector2(posX, -beginOffset - itemHeight * realHeight);
            }
            else
            {
                _rectTrans.anchoredPosition = new Vector2(posX, beginOffset + itemHeight * realHeight);
            }
        }
    }

    public void GoToTop()
    {
        SetPageIndex(0);
        IsGoTo = true;
        firstIndex = 0;
        lastIndex = maxItemCount - 1;
        mScrollRect.verticalNormalizedPosition = posList[0];
        for (int i = 0; i <= maxItemCount - 1; i++)
        {
            GameObject _obj = itemList[i];
            int index = i;
            _obj.name = index.ToString();
            refreshItem.Invoke(_obj, index, true);
            int realRow = index % (rowCount);
            int realHeight = (int)Mathf.Floor(index / rowCount);
            float posX = (float)((realRow + 1) - rowCount / 2.0 - 0.5) * ItemWidth;

            RectTransform _rectTrans = _obj.GetComponent<RectTransform>();
            if (isUpperStart == true)
            {
                _rectTrans.anchoredPosition = new Vector2(posX, -beginOffset - itemHeight * realHeight);
            }
            else
            {
                _rectTrans.anchoredPosition = new Vector2(posX, beginOffset + itemHeight * realHeight);
            }
        }
    }

    public void GoToBottom()
    {
        SetPageIndex(datasNum - 1);
        IsGoTo = true;
        firstIndex = datasNum - maxItemCount;
        lastIndex = datasNum - 1;
        mScrollRect.verticalNormalizedPosition = posList[datasNum - 3];
        for (int i = 0; i <= maxItemCount - 1; i++)
        {
            GameObject _obj = itemList[i];
            int index = i + datasNum - maxItemCount;
            _obj.name = index.ToString();
            refreshItem.Invoke(_obj, index, true);

            int realRow = index % (rowCount);
            int realHeight = (int)Mathf.Floor(index / rowCount);
            float posX = (float)((realRow + 1) - rowCount / 2.0 - 0.5) * ItemWidth;

            RectTransform _rectTrans = _obj.GetComponent<RectTransform>();
            if (isUpperStart == true)
            {
                _rectTrans.anchoredPosition = new Vector2(posX, -beginOffset - itemHeight * realHeight);
            }
            else
            {
                _rectTrans.anchoredPosition = new Vector2(posX, beginOffset + itemHeight * realHeight);
            }
        }
    }

    private void SetPageIndex(int index)
    {
        if (currentPageIndex != index)
        {
            currentPageIndex = index;
        }
    }

    public int GetPageIndex()
    {
        return currentPageIndex;
    }

    public GameObject GetCurItemGO()
    {
        GameObject selectItemGO = null;
        float offset = mScrollRect.content.rect.height;
        for (int i = 0; i <= itemList.Count - 1; i++)
        {
            GameObject _obj = itemList[i];
            RectTransform _rectTrans = _obj.GetComponent<RectTransform>();
            float temp = Mathf.Abs(_rectTrans.anchoredPosition.y + beginOffset + currentPageIndex * itemHeight);
            if (temp < offset)
            {
                selectItemGO = _obj;
                offset = temp;
            }
        }
        return selectItemGO;
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        prePosY = -1;
        isInSlide = true;
        startTime = 0;
        stopMove = true;
        IsSlidePosEqual = false;
        isDragEnd = false;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        isDragEnd = true;
    }

    public bool GetIsInSlide()
    {
        return isInSlide;
    }
}
