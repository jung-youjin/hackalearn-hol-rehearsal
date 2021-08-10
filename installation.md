`npm init gatsby`
> 맨 위 option 빼고 다 설정
> google analytics track id는 아무거나 꼭 넣어주기 - 만약 깜빡했다면 gatsby-config.js에서 수정!

`cd gasby-app`
`gatsby develop`

new terminal
`func init api` -> 1번 option dotnet => api folder가 생성됨
`cd api`

`func new`
-> http trigger (2번)
-> function name: wordcpresshttptrigger
=> api folder 안에 wordpresshttptrigger 만들어짐

`dotnet restore .` 빨간 줄 사라짐
GET 만쓸거고 POST 필요 없어서

> wordpresshttptrigger.cs file
public static Task Run 안에
``` cs
log.LogInformation("C# tRIGGER FUNCTION PROCESSED A REQUEST");

// 후에 추가
var blogUrl = Environment.GetEnvironmentVariable("BLOG_URI");
var requestUrl = $"https://public-api.wordpress.com/rest/v1.1/sites/youjinjung.wordpress.com/posts/";
//var requestUrl = $"https://public-api.wordpress.com/rest/v1.1/sites/{blogUrl}/posts/";
var json = default(string);
using (var http = new HttpClient()) {
    json = await http.GetStringAsync("https://public-api.wordpress.com/rest/v1.1/sites/youjinjung.wordpress.com/posts/");
}

// 밑에 싹다 지우고

var deserialised_response = JsonConvert.DeserializeObject<object>(json); // 전
// var deserialised_response = JsonConvert.DeserializeObject<PostCollection>(json); // 후
return new OkObjectResult(deserialised_response);
```
`dotnet build .`

`func start` -> function app이 돌아감
=> application end-point를 웹브라우저에 치면 동일한 결과가 나와야한다 (받아서 그대로 처리하니까)

**원본 데이터를 줄일 필요가 있다** => 그렇게 해야 트래픽 양도 줄어드니까

api folder 안에 Models 폴더를 만들고 file을 "PostCollection.cs" 라는 file 을 만든다

> PostCollection.cs 파일

``` cs
public class PostCollection {
    public virtual List<Post> Posts {get; set;} = new List<Post>();
}

public class Post {
    public virtual int Id {get; set;}
    public virtual string Author {get; set;}
}

public class Author {
    public virtual Author Author {get; set;}
}
```
// json 크기를 줄인다

> local.settings.json "Values" 에서 추가
``` json
"BLOG_URI" : "youjinjung.wordpress.com"
```

확인하면 똑같이 나옴

> loca.settings.json

``` json
"Hosts": {
    "CORS": "http://localhost:8000"
}
```

`CORS` 설정으로 똑같이 나오게 함

> index.js

node에서 기본으로 제공하는 fetch 방식 사용
`const IndexPage` 수정하기

``` js
import { useState, useEffect } from "react"
import fetch from "node-fetch"

const IndexPage = () => {
    // 중간에 state 저장해주는 부분 필요

    // Call API
    cost [posts, setPosts] = useState([]) // 비어있는 배열로 설정,초기화 시키는 명령어 
    // posts를 이용해 값을 set해라
    // useState는 현재 상태 설정
    // useEffect는 이 값을 갖고 렌더링할때 사용
    // useEffect(() => {}, []) // 요런식으로 호출하는 상태, 결과값이 없으면 빈 배열로 가라
    useEffect(() => {
        fetch(`http://localhost:7071/api/posts`) // promise로 받으면 then으로 json으로 파싱하기
        // fetch(`/api/posts`) 로 나중에 수정하면 json < error >
        // 304 에러는 보냈지만 원하는건 받아오지 않는 상황
        // 개츠비와 벡엔드 api간 소통이 안되는 상황
        .then(response => response.json())
        .then(result => { // 그 json 값이 result가 된다
            setPosts(result.posts) //거기에 posts로 날라오기 때문에 setPosts를 통해 posts 객체에 할당해라
        })
    }, [])
}
```

> index.js 

``` js
{posts.map(link => (
          <li key={link.url} style={{ ...listItemStyles, color: link.color }}>
            <span>
              <a style={linkStyle} href={post.url} target="_blank">{post.title}</a>
              on {post.date} by {post.author}
            </span>
          </li>
        ))}
```

> index.js

``` cs
fetch(`/api/posts`)
```

### 이제 여기서 aswa-cli가 필요

> terminal
`nvm use --lts=fermium`
`swa start http://localhost:8000 --api ./api`
localhost gatsby에 api를 붙여라 => swa cli가 이 둘을 붙여줌
`func start`도 해주어야함

**현재 터미널 총 3개**

새로운 4280번 포트로 swa가 생김
8000번 포트는 계속 에러가 남

4280번은 status posts 200으로 제대로 통신이 됨

개츠비 (프론트) awsa(백엔드) 잘 통신함

## 이제 azure에 배포할 차례

`commit` 전 해야할 중요할 점

template을 불러오는 gatsby app 폴더에서 .git이 있어 gatsby가 sub=module로 들어가기 때문에
`gatsby-app` 안에 있는 `.git`을 지운다 **root .git** 아님 주의

hackalearn-hol-rehearsal 후 custom
gatsby-app public에 배포

`var blogUrl = Environment.GetEnvironmentVariable("BLOG_URI");` 이걸 환경변수로 설정해준 것이 없으니 
**portal**에 가서, Configuration에 Add application setting
=> **BLOG_URI** 와 **블로그 주소**  **name** **value**로 넣어주기!!!

중요한 값들은 환경변수로 setting해주어야한다