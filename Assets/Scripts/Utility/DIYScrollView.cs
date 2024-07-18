using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;


//横方向
public class DIYScrollView : MonoBehaviour, IBeginDragHandler, IEndDragHandler
{

    private ScrollRect mScrollRect;
    private RectTransform mContentRect;
    private float targetHorizontal = 0;               //滑动的目标坐标
    private bool isInSlide = false;                     //是否在滑动中
    private bool isDragEnd = false;                    //是否拖拽结束
    private List<float> posList = new List<float>();//求出每页的临界角，页索引从0开始
    [SerializeField]
    private int currentPageIndex = 0;
    private bool stopMove = true;
    private float startTime;
    private float prePosX = -1;          //之前Y轴的位置
    public float smooting = 4;      //滑动速度
    public bool isAdsorb = false;   //是否吸附

    public GameObject itemPrefab;
    public float itemHeight;//item的高度（全都一样）
    public float ItemWidth;
    public int maxItemCount; //最大item生成数量，真实item生成数量的阀值
    public int datasNum;//数据数量
    public Action<GameObject, int> refreshItem;//item刷新函数


    public int rowCount;    //一排的数量
    public int beginOffset; //起始偏移量
    public int ExtraSize; //额外宽度(高度)

    private float MoveX = 0;
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


    public RectTransform GetContent()
    {
        return mContentRect;
    }

    // Use this for initialization
    void Start()
    {

        mScrollRect = transform.GetComponent<ScrollRect>();
        mContentRect = mScrollRect.content;
        mScrollRect.onValueChanged.AddListener((Vector2 vec) => OnScrollMove(vec));

        InitScroll();

        float horLength = mScrollRect.content.rect.width - GetComponent<RectTransform>().rect.width;
        if (isUpperStart)
        {
            posList.Add(1);
            for (int i = 1; i < datasNum - 1; i++)
            {
                float value = 1 - ItemWidth * i / horLength;
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
        SetContentWidth();
        InitCountent();
    }

    //设置滚动条的Content宽度
    public void SetContentWidth()
    {
        int realWidth = (int)Mathf.Ceil(datasNum / (float)rowCount);
        mContentRect.sizeDelta = new Vector2(ItemWidth * realWidth + Mathf.Abs(beginOffset) + ExtraSize, mContentRect.sizeDelta.y);
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
            refreshItem.Invoke(_obj, i);


            int realRow = i % (rowCount);
            int realWidth = (int)Mathf.Floor(i / rowCount);
            //float posY = (float)((realRow + 1) - rowCount / 2.0 - 0.5) * itemHeight;
            float posY = (float)(rowCount / 2.0 + 0.5 - (realRow + 1)) * itemHeight;

            RectTransform _rectTrans = _obj.GetComponent<RectTransform>();
            if (isUpperStart == true)
            {
                _rectTrans.pivot = new Vector2(0f, 0.5f);
                _rectTrans.anchorMax = new Vector2(0f, 0.5f);
                _rectTrans.anchorMin = new Vector2(0f, 0.5f);
                _rectTrans.anchoredPosition = new Vector2(beginOffset + ItemWidth * realWidth, posY);
            }
            else
            {
                _rectTrans.pivot = new Vector2(1f, 0.5f);
                _rectTrans.anchorMax = new Vector2(1f, 0.5f);
                _rectTrans.anchorMin = new Vector2(1f, 0.5f);
                _rectTrans.anchoredPosition = new Vector2(-beginOffset - ItemWidth * realWidth, posY);
            }

            lastIndex = i;
            itemList.Add(_obj);
        }

        firstIndex = initIndex;

        if (isUpperStart == true)
        {
            mContentRect.transform.localPosition = new Vector3(ItemWidth * initIndex, 0, 0);
        }
        else
        {
            mContentRect.transform.localPosition = new Vector3(-ItemWidth * initIndex, 0, 0);
        }
    }

    public void OnScrollMove(Vector2 pVec)
    {
        if (IsGoTo == true)
        {
            MoveX = pVec.x;
            IsGoTo = false;
            return;
        }



        if (isUpperStart == true)
        {
            //Debug.Log("测试输出 向右滑");
            //向右滑
            int Width = (int)Mathf.Floor((firstIndex) / rowCount);
            int lastWidth = (int)Mathf.Floor((lastIndex) / rowCount);
            int totalWidth = (int)Mathf.Floor((maxItemCount) / rowCount);

            //Debug.Log("测试输出 anchoredPosition x :" + mContentRect.anchoredPosition.x);
            if ((MoveX < pVec.x) && mContentRect.anchoredPosition.x < 0 && Mathf.Abs(mContentRect.anchoredPosition.x) >= ((Width + 1) * ItemWidth + beginOffset) && lastIndex != datasNum - 1)
            {
                //Debug.Log("测试输出 MoveX ： " + MoveX);
                //Debug.Log("测试输出 pVec ： " + pVec.x);
                //Debug.Log("测试输出 x :" + mContentRect.anchoredPosition.x);
                //Debug.Log("测试输出 width_x :" + ((Width + 1) * itemHeight + beginOffset));
                //Debug.Log("测试输出 Width :" + Width);

                //Debug.Log("测试输出 向右");
                int addRow = (int)Mathf.Ceil((Mathf.Abs(mContentRect.anchoredPosition.x) - ((Width + 1) * ItemWidth + beginOffset)) / ItemWidth);
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
                        //float posY = (float)((realRow + 1) - rowCount / 2.0 - 0.5) * itemHeight;
                        float posY = (float)(rowCount / 2.0 + 0.5 - (realRow + 1)) * itemHeight;
                        float posX = beginOffset + realHeight * ItemWidth;

                        //将这个物体移到最下方
                        _rectTrans.anchoredPosition = new Vector2(posX, posY);

                        //修改显示
                        _first.name = lastIndex.ToString();
                        refreshItem.Invoke(_first, lastIndex);
                    }
                }
            }

            //向左滑
            //Debug.Log("测试输出 向左 firstIndex ：" + firstIndex);

            if ((MoveX > pVec.x) && Mathf.Abs(mContentRect.anchoredPosition.x) <= ((lastWidth + 1 - totalWidth) * ItemWidth + beginOffset) && firstIndex > 0)
            {
                //Debug.Log("测试输出 向左滑");
                int addRow = (int)Mathf.Ceil((((lastWidth + 1 - totalWidth) * ItemWidth + beginOffset) - Mathf.Abs(mContentRect.anchoredPosition.x)) / ItemWidth);
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
                        //float posY = (float)((realRow + 1) - rowCount / 2.0 - 0.5) * itemHeight;
                        float posY = (float)(rowCount / 2.0 + 0.5 - (realRow + 1)) * itemHeight;
                        float posX = beginOffset + realHeight * ItemWidth;

                        //将这个物体移到最下方
                        _rectTrans.anchoredPosition = new Vector2(posX, posY);

                        _last.name = firstIndex.ToString();
                        refreshItem.Invoke(_last, firstIndex);
                    }
                }
            }

        }



        MoveX = pVec.x;
    }

    private void Update()
    {
        float offset_posx = 0.001f;
        bool isSlideEnd = false;
        if (isInSlide == true && IsSlidePosEqual == true && Mathf.Abs(mScrollRect.horizontalNormalizedPosition - prePosX) <= offset_posx)
        {
            isSlideEnd = true;
            IsSlidePosEqual = false;
        }
        IsSlidePosEqual = Mathf.Abs(mScrollRect.horizontalNormalizedPosition - prePosX) <= offset_posx;
        prePosX = mScrollRect.horizontalNormalizedPosition;

        //计算当前离第几行最近
        float posX = mScrollRect.horizontalNormalizedPosition;
        posX = posX < 1 ? posX : 1;
        posX = posX > 0 ? posX : 0;
        int index = 0;
        float offset = Mathf.Abs(posList[index] - posX);
        for (int i = 1; i < posList.Count; i++)
        {
            float temp = Mathf.Abs(posList[i] - posX);
            if (temp < offset)
            {
                index = i;
                offset = temp;
            }
        }

        content_pos = posX;
        pre_first_index = firstIndex;
        pre_last_index = lastIndex;

        SetPageIndex(index);

        if (isAdsorb && isDragEnd && isSlideEnd && stopMove == true)
        {
            targetHorizontal = posList[currentPageIndex]; //设置目标坐标，更新函数进行插值
            startTime = 0;
            stopMove = false;
            isInSlide = false;
        }

        if (!stopMove)
        {
            startTime += Time.deltaTime;
            float t = startTime * smooting;
            mScrollRect.horizontalNormalizedPosition = Mathf.Lerp(mScrollRect.horizontalNormalizedPosition, targetHorizontal, t);
            if (t >= 1)
            {
                stopMove = true;
                isDragEnd = false;
                mScrollRect.horizontalNormalizedPosition = targetHorizontal;
            }
        }
    }

    public void AdsorbToPage(int target_index)
    {
        SetPageIndex(target_index);
        targetHorizontal = posList[target_index]; //设置目标坐标，更新函数进行插值
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

        //mScrollRect.horizontalNormalizedPosition = posList[target_index];
        mScrollRect.horizontalNormalizedPosition = content_pos;
        for (int i = 0; i <= maxItemCount - 1; i++)
        {
            if (i >= itemList.Count)
            {
                break;
            }
            GameObject _obj = itemList[i];
            int index = i + firstIndex;
            _obj.name = index.ToString();
            refreshItem.Invoke(_obj, index);


            int realRow = index % (rowCount);
            int realHeight = (int)Mathf.Floor(index / rowCount);
            float posY = (float)(rowCount / 2.0 + 0.5 - (realRow + 1)) * itemHeight;
            float posX = beginOffset + realHeight * ItemWidth;

            //将这个物体移到最下方
            RectTransform _rectTrans = _obj.GetComponent<RectTransform>();
            _rectTrans.anchoredPosition = new Vector2(posX, posY);
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
        float offset = mScrollRect.content.rect.width;
        for (int i = 0; i <= itemList.Count - 1; i++)
        {
            GameObject _obj = itemList[i];
            RectTransform _rectTrans = _obj.GetComponent<RectTransform>();
            float temp = Mathf.Abs(_rectTrans.anchoredPosition.x + beginOffset + currentPageIndex * ItemWidth);
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
        prePosX = -1;
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
