[![Review Assignment Due Date](https://classroom.github.com/assets/deadline-readme-button-24ddc0f5d75046c5622901739e7c5dd533143b0c8e959d652212380cedb1ea36.svg)](https://classroom.github.com/a/FSGTCrc2)
## assignment-2-template


Odev kapsaminda sadece TransactionController uzerinde 1 tane GetByParameter apisi hazirlayiniz. Query Parameter olarak asagidaki filtre ozelliklerini ekleyiniz. 

Filtre kriterleri: AccountNumber, MinAmountCredit, MaxAmountCredit,MinAmountDebit,MaxAmountDebit,Description BeginDate,EndDate ve ReferenceNumber

AccountNumber ReferenceNumber alanlari == ile kullanilacak 

Min-Max amout alanlari aralik olarak arama yapacak.

BeginDate - EndDate alanlari aralik olarak arama yapacak.


Bu filtre kullanimi icin reposiroty katmaninda bu kriterleri dinamik olarak parametre verebilecegimiz bir fonksiyon hazirlayiniz. Bu fonksiyon gelen kriterleri execute ederek sonucu verecektir. 

GenericRepository uzerinde tek parametre alan Where func iplemantasyonunu yapip bu fonksiyona tum paramtreleri gecerek sorguyu calistiriniz. 

Generic reposiroty nin tek parametresi 

```Expression<Func<Entity, bool>> expression```  olacak. :)

### Ödevin Yapılışı
Öncelikle bizden istenilen filter methodunu IGenericRepository içerisinde imzasını atıyoruz. İstersek bu filtre sadece Transcation içinse ITranscationRepository ye imzasını atıp Transcationrepository de içine gerekli filtreleme için işlemleri yazabilirdik. Ama ödevde GenricRepository dendiği için bizde bu işlemleri IGenericRepository ve  GenericRepository içerisinde yapıcaz.

- Öncelikle filtreleme metodunun imzasını IGenericRepository içerisinde atalım.
```c#
List<Entity> GetbyFilter(Expression<Func<Entity, bool>> filter);
//Metot, bir "Expression<Func<Entity, bool>>" türünden parametre alır. Bu, LINQ (Language Integrated Query) sorgularını kullanarak veri koleksiyonları üzerinde filtreler oluşturmayı mümkün kılan C#'ın özel bir ifade türüdür.
```
- Daha sonra GenericRepository içerisinde filtreleme işleminin içini dolduralım.
```c#
 public List<Entity> GetbyFilter(Expression<Func<Entity, bool>> filter)
    {
        // Metot, LINQ sorgularını kullanarak "dbContext" nesnesinin içindeki "Entity" türünden verileri filtreler. 
      
            return dbContext.Set<Entity>().Where(filter).ToList();    
    }
```
- Controllerda istenilen GetByParameter apisini hazıralayalım.

```c#  
    [HttpGet("GetByParameter")]
    public ApiResponse<List<TransactionResponse>> GetByParameter(
     int AccountNumber,
     string ReferenceNumber,
     decimal? MinAmountCredit,
     decimal? MaxAmountCredit,
     decimal? MinAmountDebit,
     decimal? MaxAmountDebit,
     string Description,
     DateTime? BeginDate,
     DateTime? EndDate)
    {
         //burda yukarıda parametre alırken null gelebilme ihtimalleride düşünülmüştür kişi belki tarihe göre arama yapmak istemeyebilir ve onu boş bırakabilir.
        // İstenilen kriterlerin filtreleneceği bir expression oluşturuyoruz.
        Expression<Func<Transaction, bool>> expression = transaction =>
            (string.IsNullOrEmpty(AccountNumber.ToString()) || transaction.AccountNumber == AccountNumber) &&
            (string.IsNullOrEmpty(ReferenceNumber) || transaction.ReferenceNumber == ReferenceNumber) &&
            (!MinAmountCredit.HasValue || transaction.CreditAmount >= MinAmountCredit) &&
            (!MaxAmountCredit.HasValue || transaction.CreditAmount <= MaxAmountCredit) &&
            (!MinAmountDebit.HasValue || transaction.DebitAmount >= MinAmountDebit) &&
            (!MaxAmountDebit.HasValue || transaction.DebitAmount <= MaxAmountDebit) &&
            (string.IsNullOrEmpty(Description) || transaction.Description == Description) &&
            (!BeginDate.HasValue || transaction.TransactionDate >= BeginDate) &&
            (!EndDate.HasValue || transaction.TransactionDate <= EndDate);

        var entityList = repository.GetbyFilter(expression);
        var mapped = mapper.Map<List<Transaction>, List<TransactionResponse>>(entityList);
        return new ApiResponse<List<TransactionResponse>>(mapped);
    }
```
Burada istenilen değerleri parametre alarak içeride bir expression oluşturuyoruz.Bu expressionda istediğimiz koşulları lambda ile yazıyoruz.Ardından repository içindeki GetByFilter metoduna bu expressionu yollayarak istenilen bilgileri entityList içine koyuyoruz. Daha sonra derste yapılan mapleme işlemini de ekleyerek değeri geri döndürüyoruz.

İçindeki filtreleme işlemlerinden bir tanesini  örnek olarak açıklayalım böylece daha anlaşılır olur.

```c#
  (string.IsNullOrEmpty(AccountNumber.ToString()) || transaction.AccountNumber == AccountNumber)
```
- Eğer ben AccountNumber da hiç değer girmemişsem eşit olmasına bakmaksızın true döndürücek. Çünkü bunu yapmamın sebebi kişi belki filtreleme işlemi yaparken accountNumbera göre yapmak istemeyip ReferenceNumber veya tarih olarak yapıcak olabilir.O zamanda AccountNumber' ı boş bırakabilir.Zaten eğer değer girmişse buna göre filtreleme yapıcak demmiştir o yüzden  direk eşit olmasına bakıcak ve ona göre bu ifadeden true yada false dönücek .
Aynı mantık diğerleri içinde düşünülerek ona göre filtreleme işlemleri gerçekleştirilmiştir.
