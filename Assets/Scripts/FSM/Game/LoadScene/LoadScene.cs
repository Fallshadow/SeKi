using System;
using System.Collections;
using UnityEngine.SceneManagement;

namespace ASeKi.fsm
{
    public abstract class LoadScene : State<game.GameController>
    {
        private const string EMPTY_SCENE_NAME = "Empty";
        
        private IEnumerator loading;

        protected LoadScene(constants.StageType st)
        {
            
        }

        /// <summary>
        /// 开始加载场景，子类重写这个方法来进行一些独特的数据更新和加载界面
        /// </summary>
        protected abstract void onEnter();

        /// <summary>
        /// 场景关闭，子类重写这个方法来进行一些独特的数据更新和加载界面
        /// </summary>
        protected abstract void onExit();

        /// <summary>
        /// 加载场景中, 真正的场景加载会在这个消息之后
        /// </summary>
        protected abstract void onLoading();

        /// <summary>
        /// 场景加载完
        /// </summary>
        protected abstract void onLoadingEnd();

        /// <summary>
        /// 加载的场景id
        /// </summary>
        /// <returns></returns>
        protected abstract int levelId();

        public override void Enter()
        {
            ui.UiManager.instance.DestroyAllUi();//一定放在onenter前面
            onEnter();
            
            if (loading != null)
            {
                game.GameController.instance.StopCoroutine(loading);
            }

            loading = _loading();
            game.GameController.instance.StartCoroutine(loading);
        }


        public override void Exit()
        {
            onExit();

            game.GameController.instance.StopCoroutine(loading);
        }

        public override int StateEnum()
        {
            return (int)GameFsmState.LOAD_SCENE;
        }

        private IEnumerator _loading()
        {
            SceneManager.LoadScene(EMPTY_SCENE_NAME);
            yield return null;

            Utility.Common.UnLoadAllUnusedAssets();
            yield return null;

            Utility.Common.GcCollect();

            yield return null;

            onLoading();

            SceneManager.LoadSceneAsync("21.ModelViewerTool", LoadSceneMode.Single);
            yield return null;

            onLoadingEnd();
        }

    }
}