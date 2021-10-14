<?php 
	// ========================== UserItemLoad.php ==============================
	$u_id = $_POST[ "Input_user" ];

	// 나의 DB에 접근하기
	$con = mysqli_connect("localhost", "sangku2000", "Sang3155%", "sangku2000");

	if(!$con)
	{
		die("Could not Connect!!" . mysqli_connect_error());	// 서버를 빠져나가고 ("에러 메세지", 에러 내용) 출력
	}
	
	// 연결까지는 되었다......!
	
	$u_id = 1;
	$check = mysqli_query($con, "SELECT * FROM UserItem WHERE UserId = '".$u_id."'");
		// 유저 아이디가 데이터베이스에 존재하는지?
		// $u_id를 문자열로 추출하기 위해 ' " . $u_id . " '  로 만듬
		// 이러면 추출되면 ' ' 로 감싸지게 된다. 
	
	// 이제 $check 에는 Id와 관련된 모든 값이 들어가있다.
	// 여기서 몇가지 조건을 파악한 후 Usealbe만을 추출해야한다.
	// 
	
	// 데이터베이스에 쿼리를 보내 레코드를 추출한다.
	$numrows = mysqli_num_rows($check);
	if($numrows == 0)
	{  //mysqli_num_rows() 함수는 데이터베이스에서 쿼리를 보내서 나온 레코드의 개수를 알아낼 때 쓰임
	   //즉 0이라는 뜻은 해당 조건을 못 찾았다는 뜻

		die("ID does not exist. \n");
	}


	// 찾은 userid의 갯수만큼 배열에 추가한다.
	$RowDatas = array();
	for ($i = 0; $i < $numrows; $i++){
		$row = mysqli_fetch_assoc($check);
		$RowDatas[$i] = $row;
	}
	
	$result = array();
	$j = 0;
	for($i = 0; $i < $numrows; $i++){
		// 내가 구매했고, 공격팀의 유닛인 경우만 추출한다.
		if($RowDatas[$i]['isBuy'] == 1 && $RowDatas[$i]['isAttack'] == 1){
			$result[$j] = $RowDatas[$i];
			$j++;
		}
	}
	
	if(sizeof($result) == 0){
		die("Data does not exist.\n");
	}
	
	// JSON 생성 부분
	$JSONDatas = array();
	for($i = 0; $i < sizeof($result); $i++){		
		$JSONDatas[$i]["ItemNo"] = $result[$i]['ItemNo'];
		$JSONDatas[$i]["ItemName"] = $result[$i]['ItemName'];
		$JSONDatas[$i]["Level"] = $result[$i]['Level'];
		$JSONDatas[$i]["isBuy"] = $result[$i]['isBuy'];
		$JSONDatas[$i]["PosX"] = $result[$i]['PosX'];
		$JSONDatas[$i]["PosY"] = $result[$i]['PosY'];
		$JSONDatas[$i]["KindOfItem"] = $result[$i]['KindOfItem'];
		$JSONDatas[$i]["ItemUsable"] = $result[$i]['ItemUsable'];
		$JSONDatas[$i]["isAttack"] = $result['isAttack'];
	}
	$output = json_encode($JSONDatas, JSON_UNESCAPED_UNICODE);	// PHP 5.4 이상 버전에서 한글이 안깨지게 JSON_UNESCAPED_UNICODE 추가
	// => JSON 파일 생성됨

	//출력
	if($output != null)
		echo $output;
		echo "Get_ItemList_Success..!";


	// 데이터베이스를 닫아준다.
	mysqli_close($con);
?>