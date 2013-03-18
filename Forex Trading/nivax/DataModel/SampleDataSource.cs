using System;
using System.Linq;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Windows.ApplicationModel.Resources.Core;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using System.Collections.Specialized;

// The data model defined by this file serves as a representative example of a strongly-typed
// model that supports notification when members are added, removed, or modified.  The property
// names chosen coincide with data bindings in the standard item templates.
//
// Applications may use this model as a starting point and build on it, or discard it entirely and
// replace it with something appropriate to their needs.

namespace BricksStyle.Data
{
    /// <summary>
    /// Base class for <see cref="SampleDataItem"/> and <see cref="SampleDataGroup"/> that
    /// defines properties common to both.
    /// </summary>
    [Windows.Foundation.Metadata.WebHostHidden]
    public abstract class SampleDataCommon : BricksStyle.Common.BindableBase
    {
        private static Uri _baseUri = new Uri("ms-appx:///");

        public SampleDataCommon(String uniqueId, String title, String subtitle, String imagePath, String description)
        {
            this._uniqueId = uniqueId;
            this._title = title;
            this._subtitle = subtitle;
            this._description = description;
            this._imagePath = imagePath;
        }

        private string _uniqueId = string.Empty;
        public string UniqueId
        {
            get { return this._uniqueId; }
            set { this.SetProperty(ref this._uniqueId, value); }
        }

        private string _title = string.Empty;
        public string Title
        {
            get { return this._title; }
            set { this.SetProperty(ref this._title, value); }
        }

        private string _subtitle = string.Empty;
        public string Subtitle
        {
            get { return this._subtitle; }
            set { this.SetProperty(ref this._subtitle, value); }
        }

        private string _description = string.Empty;
        public string Description
        {
            get { return this._description; }
            set { this.SetProperty(ref this._description, value); }
        }

        private ImageSource _image = null;
        private String _imagePath = null;
        public ImageSource Image
        {
            get
            {
                if (this._image == null && this._imagePath != null)
                {
                    this._image = new BitmapImage(new Uri(SampleDataCommon._baseUri, this._imagePath));
                }
                return this._image;
            }

            set
            {
                this._imagePath = null;
                this.SetProperty(ref this._image, value);
            }
        }

        public void SetImage(String path)
        {
            this._image = null;
            this._imagePath = path;
            this.OnPropertyChanged("Image");
        }

        public override string ToString()
        {
            return this.Title;
        }
    }

    /// <summary>
    /// Generic item data model.
    /// </summary>
    public class SampleDataItem : SampleDataCommon
    {
        public SampleDataItem(String uniqueId, String title, String subtitle, String imagePath, String description, String content, int colSpan, int rowSpan, SampleDataGroup group)
            : base(uniqueId, title, subtitle, imagePath, description)
        {
            this._colSpan = colSpan;
            this._rowSpan = rowSpan;
            this._content = content;
            this._group = group;
        }

        private string _content = string.Empty;
        public string Content
        {
            get { return this._content; }
            set { this.SetProperty(ref this._content, value); }
        }

        private int _rowSpan = 1;
        public int RowSpan
        {
            get { return this._rowSpan; }
            set { this.SetProperty(ref this._rowSpan, value); }
        }

        private int _colSpan = 1;
        public int ColSpan
        {
            get { return this._colSpan; }
            set { this.SetProperty(ref this._colSpan, value); }
        }


        private SampleDataGroup _group;
        public SampleDataGroup Group
        {
            get { return this._group; }
            set { this.SetProperty(ref this._group, value); }
        }
    }

    /// <summary>
    /// Generic group data model.
    /// </summary>
    public class SampleDataGroup : SampleDataCommon
    {
        public SampleDataGroup(String uniqueId, String title, String subtitle, String imagePath, String description)
            : base(uniqueId, title, subtitle, imagePath, description)
        {
            Items.CollectionChanged += ItemsCollectionChanged;
        }

        private void ItemsCollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            // Provides a subset of the full items collection to bind to from a GroupedItemsPage
            // for two reasons: GridView will not virtualize large items collections, and it
            // improves the user experience when browsing through groups with large numbers of
            // items.
            //
            // A maximum of 12 items are displayed because it results in filled grid columns
            // whether there are 1, 2, 3, 4, or 6 rows displayed

            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    if (e.NewStartingIndex < 12)
                    {
                        TopItems.Insert(e.NewStartingIndex,Items[e.NewStartingIndex]);
                        if (TopItems.Count > 12)
                        {
                            TopItems.RemoveAt(12);
                        }
                    }
                    break;
                case NotifyCollectionChangedAction.Move:
                    if (e.OldStartingIndex < 12 && e.NewStartingIndex < 12)
                    {
                        TopItems.Move(e.OldStartingIndex, e.NewStartingIndex);
                    }
                    else if (e.OldStartingIndex < 12)
                    {
                        TopItems.RemoveAt(e.OldStartingIndex);
                        TopItems.Add(Items[11]);
                    }
                    else if (e.NewStartingIndex < 12)
                    {
                        TopItems.Insert(e.NewStartingIndex, Items[e.NewStartingIndex]);
                        TopItems.RemoveAt(12);
                    }
                    break;
                case NotifyCollectionChangedAction.Remove:
                    if (e.OldStartingIndex < 12)
                    {
                        TopItems.RemoveAt(e.OldStartingIndex);
                        if (Items.Count >= 12)
                        {
                            TopItems.Add(Items[11]);
                        }
                    }
                    break;
                case NotifyCollectionChangedAction.Replace:
                    if (e.OldStartingIndex < 12)
                    {
                        TopItems[e.OldStartingIndex] = Items[e.OldStartingIndex];
                    }
                    break;
                case NotifyCollectionChangedAction.Reset:
                    TopItems.Clear();
                    while (TopItems.Count < Items.Count && TopItems.Count < 12)
                    {
                        TopItems.Add(Items[TopItems.Count]);
                    }
                    break;
            }
        }

        private ObservableCollection<SampleDataItem> _items = new ObservableCollection<SampleDataItem>();
        public ObservableCollection<SampleDataItem> Items
        {
            get { return this._items; }
        }

        private ObservableCollection<SampleDataItem> _topItem = new ObservableCollection<SampleDataItem>();
        public ObservableCollection<SampleDataItem> TopItems
        {
            get {return this._topItem; }
        }
    }

    /// <summary>
    /// Creates a collection of groups and items with hard-coded content.
    /// 
    /// SampleDataSource initializes with placeholder data rather than live production
    /// data so that sample data is provided at both design-time and run-time.
    /// </summary>
    public sealed class SampleDataSource
    {
        private static SampleDataSource _sampleDataSource = new SampleDataSource();

        private ObservableCollection<SampleDataGroup> _allGroups = new ObservableCollection<SampleDataGroup>();
        public ObservableCollection<SampleDataGroup> AllGroups
        {
            get { return this._allGroups; }
        }

        public static IEnumerable<SampleDataGroup> GetGroups(string uniqueId)
        {
            if (!uniqueId.Equals("AllGroups")) throw new ArgumentException("Only 'AllGroups' is supported as a collection of groups");
            
            return _sampleDataSource.AllGroups;
        }

        public static SampleDataGroup GetGroup(string uniqueId)
        {
            // Simple linear search is acceptable for small data sets
            var matches = _sampleDataSource.AllGroups.Where((group) => group.UniqueId.Equals(uniqueId));
            if (matches.Count() == 1) return matches.First();
            return null;
        }

        public static SampleDataItem GetItem(string uniqueId)
        {
            // Simple linear search is acceptable for small data sets
            var matches = _sampleDataSource.AllGroups.SelectMany(group => group.Items).Where((item) => item.UniqueId.Equals(uniqueId));
            if (matches.Count() == 1) return matches.First();
            return null;
        }

       

        public SampleDataSource()
        {
            String ITEM_CONTENT = String.Format("Item Content: {0}\n\n{0}\n\n{0}\n\n{0}\n\n{0}\n\n{0}\n\n{0}",
                        "Curabitur class aliquam vestibulum nam curae maecenas sed integer cras phasellus suspendisse quisque donec dis praesent accumsan bibendum pellentesque condimentum adipiscing etiam consequat vivamus dictumst aliquam duis convallis scelerisque est parturient ullamcorper aliquet fusce suspendisse nunc hac eleifend amet blandit facilisi condimentum commodo scelerisque faucibus aenean ullamcorper ante mauris dignissim consectetuer nullam lorem vestibulum habitant conubia elementum pellentesque morbi facilisis arcu sollicitudin diam cubilia aptent vestibulum auctor eget dapibus pellentesque inceptos leo egestas interdum nulla consectetuer suspendisse adipiscing pellentesque proin lobortis sollicitudin augue elit mus congue fermentum parturient fringilla euismod feugiat");

            var group1 = new SampleDataGroup("Group-1",
                 "Forex Basics",
                 "Forex Basics",
                 "Assets/10.jpg",
                 "If you want to invest in forex then you should know the meaning of forex trading.The meaning of forex is foreign exchange. Foreign exchange market provides information about the different currencies. It can be used fore comparing different currencies. It is considered to be the large market of the world.");

            group1.Items.Add(new SampleDataItem("Big-Group-1-Item1",
                 "Meaning of Forex",
                 "Meaning of Forex",
                 "Assets/11.jpg",
                 "Now forex is open for the ordinary person. Even an ordinary person can plan to take the benefit of this forex and try to earn the fruits that they want to earn in limited span of time. If you want to learn about the different techniques that are adopted by the forex then you need to look at internet. There are many websites that can help you to get the fruits that you want.",
                 "If you want to invest in forex then you should know the meaning of forex trading.The meaning of forex is foreign exchange. Foreign exchange market provides information about the different currencies. It can be used fore comparing different currencies. It is considered to be the large market of the world. You can deal in cash value, governments, different speculators of the currency, multinational companies and various other financial institutes or the market.\n\nRetail traders are considered to be the small speculators. These small investors can become the part of the large market. These investors would participate with the help of the brokers or the bank. You can easily become the victim of the frauds. So you need to be aware of these scams as their main aim is to take the maximum benefit of your situation and they would try to exploit you.\n\nNow even the small investors can take part in this market and they can earn the huge profit as soon as possible. You should not take hap hazard decision in selecting the trader. If you take the haphazard decisions then you can end up making losses instead of profits. You also need to be aware of scams.\n\nNow forex is open for the ordinary person. Even an ordinary person can plan to take the benefit of this forex and try to earn the fruits that they want to earn in limited span of time. If you want to learn about the different techniques that are adopted by the forex then you need to look at internet. There are many websites that can help you to get the fruits that you want. These sites can also help you to learn the activities that are important for you to know. You are a smart investor then you would start trading with the nominal amount you would not trade with the high amount. This would help you to gain the confidence and invest smartly in the market of forex.  Thus, your confidence would aid you to invest in big amount in the market of forex trading.\n\nThere are many traders that would give you the advice to open a demo account. This account would help you to learn the different techniques of trading in forex market. You can open this account with minimal charges. You don’t need to pay high amount if you want to take the benefit of this account. After this you can open a standard account with the forex.\n\nIf you want to gain information about the forex then you should look at internet. There are many sites that would help you to collect the information that you are in need of. These sites would provide you necessary tutorials that can help you to trade on forex and earn huge profits in limited span of time. It would also help you to select the best trader for your deal. You can also compare the different traders and choose the trader as per your needs and requirements. You need to be particular in selecting the trader for yourself.",
                 79,
                 49,
                 group1));

            group1.Items.Add(new SampleDataItem("Small-Group-1-Item2",
                 "Significance of Forex Robots",
                 "Significance of Forex Robots",
                 "Assets/12.jpg",
                 "If you want to earn money then you need to know the significance of Forex robots.There are Forex robots that would not help you to gain huge profit but there are some robots that can aid you to gain your profits as soon as possible.",
                 "If you want to earn money then you need to know the significance of Forex robots.There are Forex robots that would not help you to gain huge profit but there are some robots that can aid you to gain your profits as soon as possible. The following are the importance of robots:\nIf you are searching for robots then you need to explore yourself to internet there are many robots that would help you to achieve your goals within limited span of time. Net would aid you to get the necessary records about the different types of robots. You would get the fantasy records and in reality you would not find these records. These records are not repeated in reality.\n\nYou would find that the robots contain the maximum comforts. At the back you would find that the prices are written that would help you to decide the best product for yourself. It would help you to know which product you should buy and which product you are supposed to sell. This way you can take the decision about the buying and selling the particular product. It would help you to take the perfect decision about the product that you have chosen for your consumption.\n\nThese track records are of little use and it can be used to find the profits in future. Thus, these records can be used for finding future profits. If you have seen the automated Forex trading back system then you would find that it is back tested that can be used for earning the best gains and it would be very easily sold in the market. But this would not provide you guarantee that you would be able to earn the fruits that you want to earn. Due to this reason you should always purchase the real time track record. Before purchasing the product you need to find the details of the product that you have selected for yourself. This way you would be able to select the best product.\n\nThese track records are free of charge and you don’t have to high price for this record.  You should make one thing sure that the trader that you have selected should know the different disciplines of Forex trading and should have perfect knowledge about the trading system. They should know the different ways that would help them to make gains that their customers want to earn. They should know the different moves of the market. The trader should be long lasting.\n\nYou should have the regulation and self-assurance to follow the terms and conditions of Forex trading and you should have the strength to bear the long term loss. Your confidence would help you to remain in the market for long time. Your trader should also know about the different leads of the market. You should know one important aspect of trading can that is the period of recovering the loss is high as compare to the loss that is made by the trader.",
                 53,
                 49,
                 group1));

            group1.Items.Add(new SampleDataItem("Big-Group-1-Item3",
                 "Facts about the Forex robots",
                 "Facts about the Forex robots",
                 "Assets/13.jpg",
                 "The forex robots are now gaining immense popularity among the current forex traders. This is due to the fact that the forex robots help a large number of traders in order to earn a hefty sum of profits. If you have a program that is reliable then it will save a lot of your valuable time.",
                 "The forex robots are now gaining immense popularity among the current forex traders. This is due to the fact that the forex robots help a large number of traders in order to earn a hefty sum of profits. If you have a program that is reliable then it will save a lot of your valuable time. The program does all the processes related to trading on your behalf and this in turn trims down your time and efforts. This will definitely help you to have a substantial increment in your profits. However, you must have faced the problem of differentiating the bad forex robots from the good ones. Normally, there are three forex robots that are considered to be the optimum ones in the present markets. These three forex robots are even cost effective and economical. This will save you from making heavy investments in the initial stages of forex trading. You can use it easily and start earning profits from them very quickly. \n\nThe three forex robots that are considered to be the most optimum for your trading use are mentioned below.\n1. The Forex Killer is amongst the most celebrated trading robots available in the present markets. This popular trading program is a software that generates signals. It also undertakes the task of analyzing the markets. This software also endows you with the ingress and egress prices so that you can conveniently take action on it. But, it will not place orders on your behalf with out your intervention. It does not function automatically. You by yourself have to place the trade orders and accomplish your dealings. Due to the practicality and usefulness of the Forex Killer, it has received a mixture of exceptional reviews. \n2. The Forex Funnel is a type of forex robot that is employed for the currency join up of the Japanese Yen and United States Dollars. It is very convenient as well as excessively useful. It undertakes the whole process of trading on your behalf. Not only this, it even analyses the conditions of market. It places the orders of trade in your absence. The forex funnel is being tested for an extensive period of more than four years. It has even receives a variety of outstanding reviews. The installation process of the forex funnel is not technically complicated. It is so simple that even a person at the beginners’ level can operate it with ease. \n3. The Forex Tracer is a kind of famous forex robots which is analogous to the Forex funnel. This forex robot is also very convenient. It carry outs the whole process of forex dealings for you on your behalf. It is also employed for the currency duo of Euro and United States Dollars. The forex tracer is also tested from a long period of time and is still producing desirable outcomes. It is amongst the most desirable Forex robots. Even the folks that are new to the software can operate it without any complications. \n\nAny of the aforementioned three forex robots could be employed by you in order to earn significant income from the forex markets. ",
                 79,
                 49,
                 group1));

            group1.Items.Add(new SampleDataItem("Big-Group-1-Item4",
                 "How to do the Forex currency dealing",
                 "How to do the Forex currency dealing",
                 "Assets/14.jpg",
                 "In the present time and age, it is feasible to sell or buy currency from various countries in the forex market. The forex is an abbreviation for foreign exchange. The ups and downs in the forex markets prove to be beneficial for the foreign exchange traders.",
                 "In the present time and age, it is feasible to sell or buy currency from various countries in the forex market. The forex is an abbreviation for foreign exchange. The ups and downs in the forex markets prove to be beneficial for the foreign exchange traders. The forex dealers can make large profits due to fluctuations in the currency rates of the forex markets. It is easy to capture the degree of differences in the forex trading than in the other trading as the foreign exchange market is open for twenty four hours in a day. It is only closed at Saturdays and Sundays. Nowadays, the forex market is become universal. Due to this, there are always buyers as well as sellers available in the forex market. The traders on the forex markets could be varied from each other. Most of these forex dealers are looking out for some short range gains. A majority of them are overseas investors that are in search of hedging off their investments with the long range Foreign exchange trades.\n\nThe currency deals on forex are done in the currency amounts that are known as lots. These lots are generally measured in United States dollars. The lots could be bought on a margin. The basis of the forex trades is the procedural scrutiny of the record of the price of the currency. The price of the foreign exchange currency could also depend on the study of the political environment of a specific country. The taxation policies of a country also decide the price of its currency’s exchange. The rates of the unemployed in that country are one of the decider factors of a country’s currency exchange rate. The rate of inflation and various other necessary factors of that country are the deciding factor of the exchange rate of that country’s currency. There is a range of systems for the currency dealings on Forex.\n\nThe currency trading on Forex is a gigantic market. It is estimated that the trading on daily basis is flanked by one trillion United States dollars up to one hundred and ninety million United States dollars. Due to the giganticness of the money amount of Forex, it is a next to impossible thing to manipulate the Forex markets like the other small markets are easily influenced. It is a very important aspect to note that the forex legal tender trading is not looked after by a single central organization such as the ESC. SEC stands for the Security Exchange Commission. Each and every country has a different agency to have a watch over the activities related to the forex trading.\n\nThe forex trading is beneficial for the economy of a country. Due to the foreign exchange, the economic condition of a country can escalate rapidly. The importance of dealing on the forex market is understood by a large number of people. And in the present times, more and more people are gaining interest to trade on the forex market in order to gain a hefty profit. ",
                 79,
                 49,
                 group1));

            group1.Items.Add(new SampleDataItem("Big-Group-1-Item5",
                 "Importance of Forex Trading",
                 "Importance of Forex Trading",
                 "Assets/15.jpg",
                 "Many people who don’t know the importance of Forex Diversification is considered to be the very important factor of money management. Many traders are not aware of this factor. Due to this reason they avoid to use it.",
                 "Many people who don’t know the importance of Forex Diversification is considered to be the very important factor of money management. Many traders are not aware of this factor. Due to this reason they avoid to use it. Diversification is considered to be the uncommon factor of foreign exchange.  Traders would focus on pips and they would not take time to understand the concept of diversification.\n\nIf you are bale to understand the diversification then would stay for long time in Forex. This would help you to earn maximum profit within limited span of time.\n\nThe main aim of diversification is to take the advantage of the different types of currency pair or different types of strategies. This would provide necessary protection to the traders and would allow them to earn huge profits.\n\nDiversification would aid you to keep your account alive. Due to this reason it is considered to be the most important tool of trading. There are some trading experts that are aware of this factor and they are suing it to get the desired goals.\n\nIf you are trading in EUR or USD then you need to diversify your account as soon as possible. If you are trading in one pair then too you can diversify your account. There are different methods of providing protection to your account. But before this you need to know the concept of diversification. The following are the advantages and disadvantages of diversification:\n\nAdvantages of diversification:\n\nDiversification provides net safety to its users. If you are running in loss then you don’t need to take tensions because the diversification would provide you the policy of “insurance” which you can use in your bad times. In other words it would provide protection to its customers when they are facing losses.\n\nDisadvantages of diversification:\n\nIf you practice the money management then you have to bear the loss up to 5% for each trade. If the trader becomes excited about the diversification then your risk factor would increase continuously.  Due to this reason you should keep the record of diversification as this would help you to reach your goal within limited span of time.\n\nThe following are different types of methods that are used by the diversification:\n1. Split into mini position:\nIf you are dealing in standard lot then you can split it into two mini lots.\n2. Different time frame:\nIf you are dealing in EUR or USD then you would lack maximum frame. Due to this reason you need to observe the frames keenly. You should try to take the frame in the direction of the frames that would help you to earn huge profits.\n3. Trade in Different currency:\nYou can trade in the currency pairs that are not related to each other.\nThe above are the different types of methods and from this you are supposed to select the method as per your requirements. With Forex trading you would become rich in limited span of time. Thus, you should be careful in selecting the method of diversification ",
                 79,
                 49,
                 group1));

            group1.Items.Add(new SampleDataItem("Small-Group-1-Item6",
                 "Forex – The Easiest Way to Earn Money",
                 "Forex – The Easiest Way to Earn Money",
                 "Assets/16.jpg",
                 "Forex is the easiest way to earn money.In 1997 Forex trading was found and today it is considered to be the world largest providers. Forex trading would aid you to become rich in short duration of time.",
                 "Forex is the easiest way to earn money.In 1997 Forex trading was found and today it is considered to be the world largest providers. Forex trading would aid you to become rich in short duration of time. Global Forex trading gives you an opportunity to deal in online currency trading.  You can become billions in limited span of time.\n\nGlobal forex provides its services to different countries of the world. You can access this system form any corner of the world. You just need to have the computer that has internet connection. This market is open for 24 hours. You can access it whenever you are free. You can use DealBrook FX2 software to take the maximum benefit of this market. It gives you an opportunity to price more than 60 pairs of currency. The experts of Global Forex are famous for providing its services to its customers. There are some currency news bulletins and charts are provided by the market. Global Forex trading is considered to be the best platform for the beginners and the experts of Forex market.\n\nAdvantages of Forex Trading:\n\nThe most important benefit of Forex market is that it is open for 24 hours. In today’s world it is considered to be the best liquid market. This market provides leverages of 100 to 1. This would reduce the requirement for the large capital. If you want to trade in Forex then you don’t need to pay the commission. It is commission free market. This commission free trading is available for more than 60 countries.\n\nOther most important benefit is that it is the global market and there are no restrictions of trading in this market. The market conditions would not have any effect on your profit. You can take the advantage of the best opportunities.\n\nThe small investors can also take the benefits of the opportunities that are provided the Forex market. All type of investors can take the benefit of this market. Different types of investors can take the benefit of this market. Before investing in the market you should know the basic concepts of the Forex market.\n\nIf you wan to know other importance of the market then you should explore your self to internet. If you are a beginner then it would help you to way different ways that would help you to earn huge profits and you can also make comparison between different traders and then select the trader as per your demands. You should be careful in selecting the trader. There are many traders that would try to take the advantage of your situation and they would not provide you proper guidelines to its users. They would try to exploit you by charging high rate of commission. Remember one thing that if you want to become a successful trader then you need to consult the expert of the particular field and then invest. These experts would give you necessary guidelines and would help you to select the best trader for your deal.",
                 53,
                 49,
                 group1));
            
            this.AllGroups.Add(group1);

             var group2 = new SampleDataGroup("Group-2",
                 "Benefits of Forex Trading",
                 "Benefits of Forex Trading",
                 "Assets/20.jpg",
                 "If you want to know invest in Forex trading then you need to know benefits of indicators that are used in Forex.The rates of currency changes slowly it is not a random process. But it is a slow and gradual process. The maximum rates of the currency are dependent on the confidence of the customer and on the economic strength of that country.");

             group2.Items.Add(new SampleDataItem("Big-Group-2-Item1",
                 "Benefits of Indicators That Are Used In Forex",
                 "Benefits of Indicators That Are Used In Forex",
                 "Assets/21.jpg",
                 "The importance of the fundamental economic factor is increasing continuously.	 If you focus on the economic status of the country then it would help you to create the impression about that country. FX market would decide the value of that country. 	FX would decide the value of the currency with the help of 5 different types of indicators. These indicators can be used for generating the volume and to decide the moves in the market.",
                 "If you want to know invest in Forex trading then you need to know benefits of indicators that are used in Forex.The rates of currency changes slowly it is not a random process. But it is a slow and gradual process. The maximum rates of the currency are dependent on the confidence of the customer and on the economic strength of that country. There are many factors that play an important role in determining the rate of the particular currency. These factors are watched by FX trading market. if the economic indicators change the value of the particular currency would change automatically. The currency of the country is considered to be the economic value representative of the particular country. It would represent the economic health of the country. The price of the currency would keep on fluctuating.\n\nThe importance of the fundamental economic factor is increasing continuously.	 If you focus on the economic status of the country then it would help you to create the impression about that country. FX market would decide the value of that country. 	FX would decide the value of the currency with the help of 5 different types of indicators. These indicators can be used for generating the volume and to decide the moves in the market.\n\nEconomic impact of short term and long term trading:\n\nData is not important in deciding the value of the currency. Data would help you to know the expectations of the market. Once the data is released then most of the economist would focus on the indicators that are used for analyzing the value of the currency. The economist would try to focus on different types of indicators and it would try to manipulate the different types of indicators. Short term trading are considered to be the most important short –term trading.  In short term trading you should have the capacity of finding whether the market would allow you to earn profits or you may end up making loss.\n\nIn long term trading the sudden changes would take place in the market. In this situation it is better that you invest in short term trading as this would help you to get the desired result as soon as possible. Market expectations are declared to meet the expectations of market. if you want to earn huge profit then you need to keep the record of the indicators and the changes that are taking place in the market.\n\nIf you want to gain information about the indicators then you need to explore yourself to internet. There are various websites that would help you to select the best trader and the offers that are offered by the different traders. This would help you to know the basic of Forex trading. It would also help you to know the different methods that would help you to earn large sum of amount and it would help you to become rich. You can take the maximum benefit of Forex trading. You can generate the wealth that you want.",
                 79,
                 49,
                 group2));

             group2.Items.Add(new SampleDataItem("Small-Group-2-Item2",
                 "Advantages of Forex trading system",
                 "Advantages of Forex trading system",
                 "Assets/22.jpg",
                 "You must utilize a proper stratagem if you want your forex trading system to work efficiently. It is also unambiguous to comprehend and its application is also very easy. It also helps you to gain huge profits. You must include these strategies into the forex trading strategies with the intention of earning enormous profits.",
                 "You must utilize a proper stratagem if you want your forex trading system to work efficiently. It is also unambiguous to comprehend and its application is also very easy. It also helps you to gain huge profits. You must include these strategies into the forex trading strategies with the intention of earning enormous profits.\n\nThere is no requirement of any forex robot in order to create a forex trading system. There are various breakouts that should be considered before creating a triumphant trading system of forex. Breakouts happen when the prices of commodities break to new extents on a chart of forex. You can accumulate a large number of gains if you sell or buy breakouts. This is because of the fact that these breakouts are rooted on the foundation of two realities about the price movement of forex that are not bound to change. They are mentioned below.  \n\n1. The trends in the foreign exchange markets are for longer durations. They start and carry on form the highs and lows of the new markets. \n\n2. Most of the traders of the forex markets lose due to their refusal to sell or buy breakouts. The matter of fact is that they do not want to carry on with the breakouts. This is the main reason for most of the foreign exchange traders’ losses.  \n\nThe legitimate breakout will definitely have a high odd, which will carry on in the way of a break. However, remember that all of the breaks do not continue similarly. You must select a proper break if you want to be successful. If you want to have at least two supportive tests, but normally, they have four to five tests. They do tests in some particular time periods that is stipulated in proper time spheres. It is advisable to undergo more and more tests as they are better for you. You must also keep observing the news. Most of the times, the news guide you that they are better breakouts. You may feel a little discouraged when the breakout happens. However, you must understand the fact that the breakout will not carry on for a longer time.\n\nYou should always get the odds into your favor as this is the most rational thing to undertake. The best among the breaks are supported by the momentum of rising prices. They become indicators to verify your moves. The most appreciating moves are the Stochastic and the RSI, which could be learned easily in fifty to sixty minutes. These are considered as visual pointers and they are easily applicable. You can even use them easily to enlarge your profits. They are very helpful in getting the odds into your side. \n\nOnce you have experienced a full breakout that is sustained by th4e momentum in price, you are ready to implement your trading indication. You can stop the break out in which you are currently trading. As soon as the breakout catches momentum, you can easily trace outside the general precariousness. ",
                 53,
                 49,
                 group2));

             group2.Items.Add(new SampleDataItem("Big-Group-2-Item3",
                 "Be Alert about the Forex Trading",
                 "Be Alert about the Forex Trading",
                 "Assets/23.jpg",
                 "If you want to invest in forex then you need to be alert about the forex trading.Forex trading is the filed in which you can buy one currency and sell other currency at particular point of time. You can take the advantage of the attractive opportunities. Forex is the best place to start investment.",
                 "If you want to invest in forex then you need to be alert about the forex trading.Forex trading is the filed in which you can buy one currency and sell other currency at particular point of time. You can take the advantage of the attractive opportunities. Forex is the best place to start investment. The following are the different points that can help you invest in forex trade:\n\nYou can trade forex on part – time basis:\n\nIf you are beginner then you need to learn about the different discipline of forex market. The following are the tips that can help you to invest in forex market:\n\n1.)	Start Trading with demo account:\n\nIf you a beginner then it important for you to know the different spheres of forex. You should know the basics of demo account. If you don’t need large capital for opening the demo account. But if you are train investor then you would know the different techniques that would help you to earn huge profit. If you want to open this account then you need to consult the expert that would help you to earn large profit in short duration of time. This would lower down your risk factor.\n\n2.)	Software for trading in Forex:\n\nForex is considered to be the foundation of getting success in forex trading. There are different types of software. You need to choose the software as per your needs and requirements. You need to be careful in selecting the software. Different packages would have different tools for measuring the changes that take place in forex market.  The important benefit of this tool is that it uses tools like automation. If you use this software then you can maximum profit and save some money on your investment. But if you don’t know to use this software then you may end up making losses instead of profits.\n\nBefore investing in the forex market you need to take the perfect training about this market. You should try to gain the necessary information about the forex trading. You should know the different techniques that would aid you to the fruits that you want to earn.\n\nIf you want to collect more details about forex trading then you can look at internet. There are many websites that would allow you to make investment at proper place at proper time. This would help you to know about the perfect way of investing in forex. If you want to purchase the software for forex trading then you should look at different websites this sites would help you to select the best software for your self. You can select the software as per your requirements and demands. You need to be careful in selecting the software as your software is the base for your success. You need to be aware of frauds that would try to take the benefit of your situation and they would not provide proper guidelines about the trading in forex. Due to this reason you need to find the details of the forex software that you have selected for yourself",
                 79,
                 49,
                 group2));

             group2.Items.Add(new SampleDataItem("Big-Group-2-Item4",
                 "Importance of Forex Brokers and Bonus",
                 "Importance of Forex Brokers and Bonus",
                 "Assets/24.jpg",
                 "Online Forex attracts the attention of potential buyers. If you are new investor then you can take the advantage of the Forex trade. There are different types of bonuses. You need to select the bonus as per your demands. If you want to take the advantage of the best trader then you need to understand Forex properly. Different types of traders would offer you different types of bonus.",
                 "Online Forex attracts the attention of potential buyers. If you are new investor then you can take the advantage of the Forex trade. There are different types of bonuses. You need to select the bonus as per your demands. If you want to take the advantage of the best trader then you need to understand Forex properly. Different types of traders would offer you different types of bonus. You need to understand the difference between the vast selection that are offered and the promotions that would help you to decide the offer that you have selected the offer.\n\nYou need to find the best Forex trader that would help you to earn huge profits. Forex traders would help you to earn huge profit as soon as possible. It would help you to get the rewards and you can take the advantage of the rewards. There are many traders that offer cash as reward to their customer. If you deposit the first depot then you can take the advantage of the bonus that is offered by different traders.\n\nYou should know one thing that you have to pay for every thing that you purchase. If you want to earn the advantage of the bonus then you need to take the benefit of the promotions that are offered by different traders. There are many online brokers that can help you to collect the necessary information about the different types of brokers.\n\nIf you want to know the different types of bonuses then you need to know the requirements of the free cash.  Wagering is called as the online gambling industry and it is the part of Forex rules and regulations. There are some traders that do to agree the terms and conditions of the Forex trading.\n\nThe following are some terms and conditions that would help you to select the best bonus for your deal:\n1.	You must buy or sell minimum 10 mini lots form your real – money account. Forex traders would not be able to withdraw money from their account.\n2.	If your account is not redeemed then you would have difficulty in withdrawing money from their account. The amount that you have not withdrawn would remain in account. It would not change.\n3.	 You cannot abuse bonus that is offered by the trader. Bonuses are given according to the account of the customer, person and the environment in which the computers of the users are shared.\nBefore purchasing the product you need to know the terms and conditions of the program that you have selected for yourself. There are different types of bonus. Welcome bonus is considered to be the best bonus as this would help you to earn the maximum profit in short duration of time. It is the common method that is used by the traders. It is considered to be platform of Forex trading.\nThere are many brokers that would offer you bonuses called Loyalty Bonus. You can also take the advantage of monthly cash bonus.",
                 79,
                 49,
                 group2));

             group2.Items.Add(new SampleDataItem("Big-Group-2-Item5",
                 "Guidelines to learn trading on Forex",
                 "Guidelines to learn trading on Forex",
                 "Assets/25.jpg",
                 "A majority of the marketers that trades on the internet are aware of forex dealings. The forex dealings are also termed as online legal tender trading. Though, most of the individuals are still unaware about the dealing methods while trading on forex.",
                 "A majority of the marketers that trades on the internet are aware of forex dealings. The forex dealings are also termed as online legal tender trading. Though, most of the individuals are still unaware about the dealing methods while trading on forex. They even do not know the functionalities of forex and the place from where they can learn the basics of trading on forex. \n\nIf you aim to turn out to be a thriving trader on forex, then you must get aware of the fundamentals of forex dealing. You must learn the methods of trading on forex appositely. It is advisable to seek advices from the experts and professionals, so that, you attain adequate acquaintance about the forex dealing. There is a tutorial available for forex dealing, that you can approach. Nowadays, there are various forex corporations that bestow you with guides and tutorials on the internet. These online guides are very advantageous.\n\nThe technicalities of a forex market are explained in detail by the online foreign exchange tutorial. This tutorial will also guide you about the forms of orders that you can avail being a foreign exchange trader. A tutorial of forex would guide you about the mechanical pointers of foreign exchange and their meaning. The indicators pertaining to economy are also of vital importance. The tutorial also directs you about the alternates and tactics that could be implemented by you being a trader on forex. \n\nBefore investing your well merited money into forex, you must be fully aware of the fundamentals of forex dealings. If you are at the beginner’s level in the field of forex dealings, then it becomes crucial for you to gain knowledge of dealings in forex. There are various online companies that guide you about forex dealings and the training which they provide is hundred per cent free of cost. This is very cost effective for you as you do not have to lose any money; instead you get tips on how to accumulate profits. These websites also demonstrate you in matters pertaining to forex. Other than the online manifestations and guidance, there is a choice of trading programs available. These programs prove to be an easy way to become skilled for forex dealings. Even after some considerable time, if you want to refer these courses, they will definitely prove helpful to you. You must learn trading on forex because it teaches you to carry on trade productively. \n\nWell, trading on forex is not a difficult task unless and until you have the sufficient knowledge about forex dealings. It is advisable for you to study or take classes before investing your hard earned wealth into the forex markets. You must refer the forex charts and forex spreads in order to deal properly in the foreign exchange markets. If you have learnt all your lessons appositely, then you can earn sufficient profits from the forex dealings. However, if your knowledge about the forex markets is not sufficient, then you will definitely incur a loss in your dealings. ",
                 79,
                 49,
                 group2));

             group2.Items.Add(new SampleDataItem("Small-Group-2-Item6",
                 "Tips for escalating forex profits",
                 "Tips for escalating forex profits",
                 "Assets/26.jpg",
                 "Most of the traders that have made profit by trading on forex have an undisclosed secret with them. They do not reveal their secret with others in order to have a monopoly. If you think the above mentioned tow lines are true then you are wrong.",
                 "Most of the traders that have made profit by trading on forex have an undisclosed secret with them. They do not reveal their secret with others in order to have a monopoly. If you think the above mentioned tow lines are true then you are wrong. Frankly speaking, there are no such secrets to gain success in the forex dealings. A majority of the counselors that claim their own selves to be the trade guru are all bogus. In fact, they are a group of traders that have failed to attain success in the forex dealings and they do not have a clear idea about successful forex dealings. \n\nThere is a normal human tendency that we all want to acquire profits, but do not want to spend anything for that. If investing into forex is an easy task, then more and more individuals would definitely invest into it. There are even kinds of individuals that invest into things that are outlandish to even them. However, then also they expect to earn profits from such investments. The greediness of these folks is responsible for their failure. It is seen from the past evidences that due to greediness, most of the human beings have failed in their targets. However, there is definitely a way out for you so that you can earn a healthy amount of profit and that too with out shedding out much of your perspiration. Hence, if you want to earn something, then you should not hesitate to put in the necessary efforts. You will surely succeed in your goals. \n\nIf you want guaranteed profits while trading on forex, then there is a way for that too. There are some important tips that would escalate your profits while dealing on forex. The most significant tip is that you should never anticipate excessive from the marketplace than it is capable of providing you.\n\nYou can understand this term more widely if you are an experienced trader. If there is not doing well, you must also go short with it. Even if you earn profit with your strategies, you must bear in mind that the market is not much productive. During this period a majority of the traders choose to go short. \n\nIn spite of the gains and losses, you should never dishonor the golden tip of trading, which is not to anticipate much from the trading place than what it is capable of providing you. Those forex traders that had decided to be short have experienced a lot of threat factors. If you do not have full confidence in your dealings then merely gambling with your fate is not a rational decision. It would become a very shameful condition for you, if you gamble again and again with your destiny. \n\nAs a trader, you must have a desire for a wonderful deal that would earn you a huge profit. However, if you become more and more greedy, then it would prove to be very hazardous for you. You must play safe rather than taking unnecessary chances. ",
                 53,
                 49,
                 group2));
            
            this.AllGroups.Add(group2);
			
           
        }
    }
}
